using NLog;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactiveServices.MessageBus.RabbitMQ
{
    public class RabbitMQReceivingBus : RabbitMQMessageBus, IReceivingBus
    {
        public RabbitMQReceivingBus()
        {
            IgnoreMessagesOlderThanSubscriptionTime = false;
        }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<Tuple<Type, SubscriptionId>, RabbitMQSubscription> ActiveSubscriptions = new Dictionary<Tuple<Type, SubscriptionId>, RabbitMQSubscription>();

        public bool IsListenningTo(Type messageType, SubscriptionId subscriptionId)
        {
            return ExistingSubscriptionFor(messageType, subscriptionId) != null;
        }

        protected bool IgnoreMessagesOlderThanSubscriptionTime { get; set; }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void Receive(Type messageType, SubscriptionId subscriptionId, Action<object> messageHandler, SubscriptionMode subscriptionMode = SubscriptionMode.Shared)
        {
            TrySubscribeTo(messageType, subscriptionId, messageHandler, subscriptionMode);
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void Receive<TMessage>(SubscriptionId subscriptionId, Action<object> messageHandler, SubscriptionMode subscriptionMode = SubscriptionMode.Shared) where TMessage : class, new()
        {
            TrySubscribeTo(typeof(TMessage), subscriptionId, messageHandler, subscriptionMode);
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void RemoveSubscription(Type messageType, SubscriptionId subscriptionId)
        {
            var subscription = ExistingSubscriptionFor(messageType, subscriptionId);
            if (subscription != null)
                RemoveSubscription(subscription);
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void RemoveSubscriptions(SubscriptionId subscriptionId)
        {
            Tuple<Type, SubscriptionId>[] subscriptionsSharingTheGivenId;
            Log.Debug("Acquired lock to ActiveSubscriptions at method RemoveSubscriptions(...)");
            lock (ActiveSubscriptions)
            {
                subscriptionsSharingTheGivenId = ActiveSubscriptions.Where(s => s.Key.Item2 == subscriptionId).Select(s => s.Key).ToArray();
            }
            Log.Debug("Released lock to ActiveSubscriptions at method RemoveSubscriptions(...)");

            foreach (var subscription in subscriptionsSharingTheGivenId)
            {
                RemoveSubscription(subscription.Item1, subscription.Item2);
            }
        }

        private void RemoveSubscription(Subscription subscription)
        {
            if (subscription == null)
                return;

            Log.Debug("Acquired lock to ActiveSubscriptions at method RemoveSubscription(...)");
            lock (ActiveSubscriptions)
            {
                ActiveSubscriptions.Remove(new Tuple<Type, SubscriptionId>(subscription.MessageType, subscription.SubscriptionId));
            }
            Log.Debug("Released lock to ActiveSubscriptions at method RemoveSubscription(...)");

            // Exclusive queues are already deleted by rabbitmq when the channel is closed
            //if (subscription.SubscriptionMode == SubscriptionMode.Exclusive)
            //    DeleteSubscriptionQueue(subscription.MessageType, subscription.SubscriptionId);

            subscription.Dispose();
        }

        public void DeleteSubscriptionQueue(Type messageType, SubscriptionId subscriptionId)
        {
            var queueName = QueueNameFor(messageType, subscriptionId);
            if (QueueExists(queueName))
                DeleteQueue(queueName);
        }

        [Log]
        [LogException]
        private void TrySubscribeTo(Type messageType, SubscriptionId subscriptionId, Action<object> messageHandler, SubscriptionMode subscriptionMode)
        {
            if (subscriptionId == null) throw new ArgumentNullException("subscriptionId");
            if (messageHandler == null) throw new ArgumentNullException("messageHandler");

            var subscription = SubscriptionFor(messageType, subscriptionId, messageHandler, subscriptionMode);
            subscription.Start();
        }

        private void PrepareSubscription(RabbitMQSubscription subscription)
        {
            using (var model = NewChannel())
            {
                DeclareQueue(model, subscription);
            }

            Log.Info("Prepared to receive messages from queue '{0}'", subscription.QueueName);
        }

        private RabbitMQSubscription SubscriptionFor(Type messageType, SubscriptionId subscriptionId, Action<object> messageHandler, SubscriptionMode subscriptionMode)
        {
            var subscription = ExistingSubscriptionFor(messageType, subscriptionId);
            if ((subscription != null) && (subscription.MessageHandler != null)) //MessageHanlder == null indicates the subscription was prepared, but not yet performed
                throw new InvalidOperationException(String.Format("There is already a subscription for message type '{0}' and subscription id {1}", messageType, subscriptionId));

            subscription = NewSubscriptionFor(messageType, subscriptionId, subscriptionMode, !IgnoreMessagesOlderThanSubscriptionTime, messageHandler);

            return subscription;
        }

        private RabbitMQSubscription ExistingSubscriptionFor(Type messageType, SubscriptionId subscriptionId)
        {
            Log.Debug("Acquired lock to ActiveSubscriptions at method ExistingSubscriptionFor(...)");
            try
            {
                lock (ActiveSubscriptions)
                {
                    RabbitMQSubscription subscription;
                    ActiveSubscriptions.TryGetValue(new Tuple<Type, SubscriptionId>(messageType, subscriptionId), out subscription);
                    return subscription;
                }
            }
            finally
            {
                Log.Debug("Released lock to ActiveSubscriptions at method ExistingSubscriptionFor(...)");
            }
        }

        private RabbitMQSubscription NewSubscriptionFor(Type messageType, SubscriptionId subscriptionId, SubscriptionMode subscriptionMode, bool acceptMessagesOlderThanSubscriptionTime, Action<object> messageHandler)
        {
            var queueName = QueueNameFor(messageType, subscriptionId);
            var subscription = NewReceivingSubscription(messageType, subscriptionId, subscriptionMode, acceptMessagesOlderThanSubscriptionTime, messageHandler, queueName);

            if (IsDisposing)
                throw new InvalidOperationException("Cannot create subscriptions after the ReceivingBus be disposed!");

            Log.Debug("Acquired lock to ActiveSubscriptions at method NewSubscriptionFor(...)");
            lock (ActiveSubscriptions)
            {
                ActiveSubscriptions[new Tuple<Type, SubscriptionId>(messageType, subscriptionId)] = subscription;
            }
            Log.Debug("Released lock to ActiveSubscriptions at method NewSubscriptionFor(...)");

            return subscription;
        }

        protected virtual RabbitMQSubscription NewReceivingSubscription(Type messageType, SubscriptionId subscriptionId, SubscriptionMode subscriptionMode,
            bool acceptMessagesOlderThanSubscriptionTime, Action<object> messageHandler, string queueName)
        {
            var subscription = new RabbitMQSubscription(
                this, subscriptionId, TopicId.None, messageType, queueName, subscriptionMode, 
                acceptMessagesOlderThanSubscriptionTime, messageHandler, null, null, null);
            return subscription;
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void PrepareSubscriptionTo<TMessage>(SubscriptionId subscriptionId) where TMessage : class, new()
        {
            PrepareSubscriptionTo(typeof(TMessage), subscriptionId);
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public void PrepareSubscriptionTo(Type messageType, SubscriptionId subscriptionId)
        {
            var subscription = NewSubscriptionFor(messageType, subscriptionId, SubscriptionMode.Shared, true, null);
            PrepareSubscription(subscription);
        }

        private bool IsDisposing;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposing = true;

                Log.Debug("Acquired lock to ActiveSubscriptions at method Dispose(...)");
                lock (ActiveSubscriptions)
                {
                    while (ActiveSubscriptions.Count > 0)
                    {
                        RemoveSubscription(ActiveSubscriptions.First().Value);
                    }
                }
                Log.Debug("Released lock to ActiveSubscriptions at method Dispose(...)");
            }
        }
    }
}
