using NLog;
using PostSharp.Patterns.Diagnostics;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ReactiveServices.MessageBus.RabbitMQ
{
    [Log(AttributeExclude = true)]
    [LogException(AttributeExclude = true)]
    public class RabbitMQSubscription : Subscription
    {
        public RabbitMQSubscription(
            IRabbitMQMessageBus messageBus, SubscriptionId subscriptionId, TopicId topicId, 
            Type messageType, string queueName, SubscriptionMode subscriptionMode, bool acceptMessagesOlderThanSubscriptionTime, 
            Action<object> messageHandler, 
            Action<object, object> messageHandlerWithProperties, 
            Action<object, Dictionary<string, string>> messageHandlerWithHeaders, 
            Action<object, object, Dictionary<string, string>> messageHandlerWithPropertiesAndHeaders)
            : base(subscriptionId, topicId, messageType, queueName, subscriptionMode, acceptMessagesOlderThanSubscriptionTime, messageHandler, messageHandlerWithProperties, messageHandlerWithHeaders, messageHandlerWithPropertiesAndHeaders)
        {
            if (topicId == null) throw new ArgumentNullException("topicId");

            MessageBus = messageBus;
        }

        private IRabbitMQMessageBus MessageBus { get; set; }
        private RabbitMQConsumer Consumer { get; set; }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                DisposeConsumer();
        }

        private void DisposeConsumer()
        {
            if (Consumer != null)
            {
                Consumer.ConsumerCancelled -= OnConsumerCancelled;
                Consumer.Dispose();
                Consumer = null;
            }
        }

        internal void Start()
        {
            Consumer = NewConsumer(MessageBus);
            Consumer.ConsumerCancelled += OnConsumerCancelled;
            Consumer.Start();
        }

        protected virtual RabbitMQConsumer NewConsumer(IRabbitMQMessageBus messageBus)
        {
            return new RabbitMQConsumer(this, messageBus);
        }

        private void OnConsumerCancelled(object sender, ConsumerEventArgs args)
        {
            Stop();
            TryRestartConsuming();
        }

        private void TryRestartConsuming()
        {
            var consumerRestartedWithSuccess = false;

            for (var retry = 0; retry < 6; retry++) // Wait up to 16 seconds before giving up of the reconnection process
            {
                var interval = Math.Pow(2, retry);
                Thread.Sleep(TimeSpan.FromSeconds(interval));

                if (MessageBus.QueueExists(QueueName))
                {
                    try
                    {
                        Start();
                        consumerRestartedWithSuccess = true;
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Error trying to reconnect subscription after connection lost!");
                    }
                    break;
                }
            }

            if (!consumerRestartedWithSuccess)
                throw new TimeoutException("Could not restart subscription within 16 seconds after connection lost!");
        }

        private void Stop()
        {
            DisposeConsumer();
        }

    }
}
