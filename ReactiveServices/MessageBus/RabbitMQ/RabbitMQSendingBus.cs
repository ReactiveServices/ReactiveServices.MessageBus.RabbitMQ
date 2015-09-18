using NLog;
using PostSharp.Patterns.Diagnostics;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace ReactiveServices.MessageBus.RabbitMQ
{
    public class RabbitMQSendingBus : RabbitMQMessageBus, ISendingBus
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<ulong, bool> AcknoledgedPublishConfirmations = new Dictionary<ulong, bool>();

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void Send<TMessage>(
            TMessage message, SubscriptionId subscriptionId, 
            StorageType storageType = StorageType.Persistent, 
            bool waitForPublishConfirmation = true, 
            TimeSpan publishConfirmationTimeout = default(TimeSpan),
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan))
            where TMessage : class
        {
            TrySend(typeof(TMessage), message, subscriptionId, storageType, waitForPublishConfirmation,
                publishConfirmationTimeout, headers, expiration);
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void Send(
            Type messageType, object message, SubscriptionId subscriptionId, 
            StorageType storageType = StorageType.Persistent, 
            bool waitForPublishConfirmation = true, 
            TimeSpan publishConfirmationTimeout = default(TimeSpan),
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan))
        {
            TrySend(messageType, message, subscriptionId, storageType, waitForPublishConfirmation, 
                publishConfirmationTimeout, headers, expiration);
        }

        [Log]
        [LogException]
        protected virtual void TrySend(
            Type messageType, object message, SubscriptionId subscriptionId, 
            StorageType storageType, bool waitForPublishConfirmation, TimeSpan publishConfirmationTimeout, 
            Dictionary<string, string> headers, TimeSpan expiration)
        {
            ulong deliveryTag = 0;
            try
            {
                if (subscriptionId == null) throw new ArgumentNullException("subscriptionId");
                if (message == null) throw new ArgumentNullException("message");

                if (publishConfirmationTimeout == default(TimeSpan))
                    publishConfirmationTimeout = TimeSpan.FromSeconds(30);

                var destinationQueueName = QueueNameFor(messageType, subscriptionId);
                var messageBody = Serializer.Serialize(messageType, message);

                //About the Publish Confirms, see this http://www.rabbitmq.com/blog/2011/02/10/introducing-publisher-confirms/

                using (var model = NewChannel())
                {
                    var props = model.CreateBasicProperties();
                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            props.Headers.Add(header.Key, header.Value);
                        }
                    }
                    SetMessageExpirationTimespan(props, expiration);

                    props.DeliveryMode = storageType == StorageType.NonPersistent ? (byte)1 : (byte)2;

                    if (waitForPublishConfirmation)
                    {
                        model.ConfirmSelect();
                        model.BasicAcks += ReceiveAckForPublishing;
                        model.BasicNacks += ReceiveNackForPublishing;
                        deliveryTag = model.NextPublishSeqNo;
                    }

                    bool? acknoledgedResult = null;
                    try
                    {
                        lock (AcknoledgedPublishConfirmations)
                        {
                            AcknoledgedPublishConfirmations.Remove(deliveryTag);
                        }

                        model.BasicPublish("", destinationQueueName, props, messageBody);

                        Log.Info("Message with delivery tag '{0}' sent to queue '{1}'", deliveryTag, destinationQueueName);
                        Log.Debug("Message with delivery tag '{0}': '{1}'", deliveryTag, Encoding.UTF8.GetString(messageBody));

                        if (!waitForPublishConfirmation)
                            return;

                        var sw = new Stopwatch();
                        sw.Start();
                        while (sw.Elapsed < publishConfirmationTimeout)
                        {
                            Thread.Sleep(10);

                            if (model.IsClosed)
                            {
                                Log.Error("Model shutdown while waiting for send confirmation for message of type '{0}' and delivery tag {1}!", messageType.Name, deliveryTag);
                                break;
                            }

                            lock (AcknoledgedPublishConfirmations)
                            {
                                bool acknoledgedResultValue;
                                if (!AcknoledgedPublishConfirmations.TryGetValue(deliveryTag, out acknoledgedResultValue))
                                    continue;

                                acknoledgedResult = acknoledgedResultValue;
                                AcknoledgedPublishConfirmations.Remove(deliveryTag);
                            }
                            break;
                        }
                        sw.Stop();
                    }
                    finally
                    {
                        if (waitForPublishConfirmation)
                        {
                            model.BasicAcks -= ReceiveAckForPublishing;
                            model.BasicNacks -= ReceiveNackForPublishing;
                        }
                    }

                    if (!acknoledgedResult.HasValue)
                        throw new NoPublishConfirmationResponseForPublishedMessageException(messageType.Name);

                    if (!acknoledgedResult.GetValueOrDefault())
                        throw new NackReceivedAsPublishConfirmationResponseForPublishedMessageException(messageType.Name);

                    Log.Info("Sending confirmed for message with delivery tag '{0}'", deliveryTag);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Could not send message of type '{0}' and delivery tag {1}!", messageType.Name, deliveryTag);
                Log.Debug("Message of type '{0}' and delivery tag {1}!", messageType.Name, deliveryTag);
                throw;
            }
        }

        private void ReceiveNackForPublishing(object sender, BasicNackEventArgs args)
        {
            lock (AcknoledgedPublishConfirmations)
            {
                AcknoledgedPublishConfirmations[args.DeliveryTag] = false;
            }
        }

        private void ReceiveAckForPublishing(object sender, BasicAckEventArgs args)
        {
            lock (AcknoledgedPublishConfirmations)
            {
                AcknoledgedPublishConfirmations[args.DeliveryTag] = true;
            }
        }
    }
}
