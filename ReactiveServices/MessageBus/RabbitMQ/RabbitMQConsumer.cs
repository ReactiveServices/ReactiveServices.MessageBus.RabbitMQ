using NLog;
using RabbitMQ.Client;
using ReactiveServices.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace ReactiveServices.MessageBus.RabbitMQ
{
    public class RabbitMQConsumer : DefaultBasicConsumer, IDisposable
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        internal RabbitMQConsumer(RabbitMQSubscription subscription, IRabbitMQMessageBus messageBus)
        {
            Subscription = subscription;
            MessageBus = messageBus;
            Model = MessageBus.NewChannel();
        }

        private IRabbitMQMessageBus MessageBus { get; set; }
        private Subscription Subscription { get; set; }

        public void Start()
        {
            //Must use the same model, otherwise the SubscriptionMode.Exclusive would not work
            RabbitMQSubscriptionBus.PrepareSubscription(Model, Subscription);

            Model.BasicQos(0, 1, false);
            Model.BasicConsume(Subscription.QueueName, false, this);

            WaitStartConsuming();

            Log.Info("Start consuming queue '{0}'", Subscription.QueueName);
        }

        private void WaitStartConsuming()
        {
            var timeout = TimeSpan.FromSeconds(10);
            var sw = new Stopwatch();
            sw.Start();
            while (!IsRunning)
            {
                if (sw.Elapsed > timeout)
                    break;

                Thread.Sleep(10);
            }
            sw.Stop();

            if (!IsRunning)
                throw new TimeoutException(String.Format("Could not subscribe to queue {0} within {1} second(s)!", Subscription.QueueName, timeout.TotalSeconds));
        }

        protected static readonly IMessageSerializer Serializer = DependencyResolver.Get<IMessageSerializer>();

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            if (!Model.IsOpen) return;

            var messageObject = Serializer.Deserialize(body, Subscription.MessageType);
            var propertyHeaders = properties.Headers;
            var headers = new Dictionary<string, string>();
            if (propertyHeaders != null)
            {
                foreach (var propertyHeader in propertyHeaders)
                {
                    headers.Add(propertyHeader.Key, Encoding.UTF8.GetString((byte[])propertyHeader.Value));
                }
            }

            Log.Info("Executing handler for delivery tag '{0}' from queue '{1}'", deliveryTag, Subscription.QueueName);
            Log.Debug("Message with delivery tag '{0}': '{1}'", deliveryTag, Encoding.UTF8.GetString(body));

            try
            {
                ExecuteSubscriptionMessageHandler(properties, messageObject, headers);

                Model.BasicAck(deliveryTag, false);
            }
            catch (Exception e)
            {
                Model.BasicNack(deliveryTag, false, true);

                Log.Error(e, "Exception executing message handler for subscription '{0}'!", Subscription.SubscriptionId);

                throw;
            }
        }

        protected virtual void ExecuteSubscriptionMessageHandler(IBasicProperties properties, object messageObject, Dictionary<string, string> headers)
        {
            if (Subscription.MessageHandlerWithPropertiesAndHeaders != null)
                Subscription.MessageHandlerWithPropertiesAndHeaders(messageObject, properties, headers);

            if (Subscription.MessageHandlerWithProperties != null)
                Subscription.MessageHandlerWithProperties(messageObject, properties);

            if (Subscription.MessageHandlerWithHeaders != null)
                Subscription.MessageHandlerWithHeaders(messageObject, headers);

            if (Subscription.MessageHandler != null)
                Subscription.MessageHandler(messageObject);
        }

        public void Dispose()
        {
            Model.Close();
            Model.Dispose();

            Log.Info("Stop consuming queue '{0}'", Subscription.QueueName);
        }
    }
}
