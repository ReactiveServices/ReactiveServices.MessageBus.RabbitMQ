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
    public class RabbitMQPublishingBus : RabbitMQMessageBus, IPublishingBus
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<ulong, bool> AcknoledgedPublishConfirmations = new Dictionary<ulong, bool>();

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void Publish(
            Type messageType, object message, 
            StorageType storageType = StorageType.Persistent, 
            bool waitForPublishConfirmation = true, 
            TimeSpan publishConfirmationTimeout = default(TimeSpan),
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan))
        {
            TryPublish(messageType, TopicId.Default, message, storageType, waitForPublishConfirmation, 
                publishConfirmationTimeout, headers, expiration);
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void Publish(
            Type messageType, TopicId topicId, object message, 
            StorageType storageType = StorageType.Persistent, 
            bool waitForPublishConfirmation = true, 
            TimeSpan publishConfirmationTimeout = default(TimeSpan),
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan))
        {
            TryPublish(
                messageType, topicId, message, storageType, waitForPublishConfirmation, 
                publishConfirmationTimeout, headers, expiration);
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void Publish<TMessage>(
            TMessage message, StorageType storageType = StorageType.Persistent, 
            bool waitForPublishConfirmation = true, 
            TimeSpan publishConfirmationTimeout = default(TimeSpan),
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan)) where TMessage : class
        {
            TryPublish(message.GetType(), TopicId.Default, message, storageType, waitForPublishConfirmation, 
                publishConfirmationTimeout, headers, expiration);
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void Publish<TMessage>(
            TopicId topicId, TMessage message, 
            StorageType storageType = StorageType.Persistent, 
            bool waitForPublishConfirmation = true, 
            TimeSpan publishConfirmationTimeout = default(TimeSpan),
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan)) where TMessage : class
        {
            TryPublish(message.GetType(), topicId, message, storageType, waitForPublishConfirmation, 
                publishConfirmationTimeout, headers, expiration);
        }

        public void DeletePublishingExchange(Type messageType)
        {
            var exchangeName = ExchangeNameFor(messageType);
            if (ExchangeExists(exchangeName))
                DeleteExchange(exchangeName);
        }

        [Log]
        [LogException]
        protected virtual void TryPublish(
            Type messageType, TopicId topicId, object message, StorageType storageType, 
            bool waitForPublishConfirmation, TimeSpan publishConfirmationTimeout,
            Dictionary<string, string> headers,
            TimeSpan expiration)
        {
            ulong deliveryTag = 0;
            try
            {
                if (topicId == null) throw new ArgumentNullException("topicId");
                if (message == null) throw new ArgumentNullException("message");

                if (publishConfirmationTimeout == default(TimeSpan))
                    publishConfirmationTimeout = TimeSpan.FromSeconds(30);

                var exchangeName = ExchangeNameFor(messageType);
                var routingKey = RoutingKeyFor(topicId);
                var messageBody = Serializer.Serialize(messageType, message);

                //About the Publish Confirms, see this http://www.rabbitmq.com/blog/2011/02/10/introducing-publisher-confirms/

                using (var model = NewChannel())
                {
                    var props = model.CreateBasicProperties();
                    props.DeliveryMode = storageType == StorageType.NonPersistent ? (byte)1 : (byte)2;
                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            props.Headers.Add(header.Key, header.Value);
                        }
                    }
                    SetMessageExpirationTimespan(props, expiration);

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
                        model.ExchangeDeclare(exchangeName, "topic");
                        model.BasicPublish(exchangeName, routingKey, props, messageBody);

                        Log.Info("Message with delivery tag '{0}' published to exchange '{1}' with routing key '{2}'",
                            deliveryTag, exchangeName, routingKey);
                        Log.Debug("Message with delivery tag '{0}': '{1}'", deliveryTag,
                            Encoding.UTF8.GetString(messageBody));

                        if (!waitForPublishConfirmation)
                            return;

                        var sw = new Stopwatch();
                        sw.Start();
                        while (sw.Elapsed < publishConfirmationTimeout)
                        {
                            Thread.Sleep(10);

                            if (model.IsClosed)
                            {
                                Log.Error(
                                    "Model shutdown while waiting for publish confirmation for message of type '{0}' and delivery tag {1}!",
                                    messageType.Name, deliveryTag);
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

                    Log.Info("Publish confirmed for message with delivery tag '{0}'", deliveryTag);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Could not publish message of type '{0}' and delivery tag {1}!", messageType.Name, deliveryTag);
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
