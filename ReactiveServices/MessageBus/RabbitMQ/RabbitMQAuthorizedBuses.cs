using RabbitMQ.Client;
using ReactiveServices.Authorization;
using System;
using System.Collections.Generic;

namespace ReactiveServices.MessageBus.RabbitMQ
{
    class RabbitMQAuthorizer
    {
        public static void Verify(AuthorizedMessageOperation operation, Type messageType, object message, Dictionary<string, string> headers)
        {
            if (!headers.ContainsKey(Authorizer.AuthenticationTokenKey))
                throw new AuthorizationException(Authorizer.AuthorizationDomain, operation.ToString(), messageType.FullName, AuthorizationError.InvalidAuthenticationToken);

            var authenticationToken = headers[Authorizer.AuthenticationTokenKey];
            Authorizer.Verify(operation.ToString(), messageType.FullName, authenticationToken);
        }
    }

    public class RabbitMQAuthorizedSubscription : RabbitMQSubscription
    {
        public RabbitMQAuthorizedSubscription(IRabbitMQMessageBus messageBus, AuthorizedMessageOperation operation, SubscriptionId subscriptionId, TopicId topicId, Type messageType, string queueName, SubscriptionMode subscriptionMode, bool acceptMessagesOlderThanSubscriptionTime, Action<object> messageHandler, Action<object, object> messageHandlerWithProperties, Action<object, Dictionary<string, string>> messageHandlerWithHeaders, Action<object, object, Dictionary<string, string>> messageHandlerWithPropertiesAndHeaders) 
            : base(messageBus, subscriptionId, topicId, messageType, queueName, subscriptionMode, acceptMessagesOlderThanSubscriptionTime, messageHandler, messageHandlerWithProperties, messageHandlerWithHeaders, messageHandlerWithPropertiesAndHeaders)
        {
            Operation = operation;
        }

        private readonly AuthorizedMessageOperation Operation;

        protected override RabbitMQConsumer NewConsumer(IRabbitMQMessageBus messageBus)
        {
            return new RabbitMQAuthorizedConsumer(this, Operation, messageBus);
        }
    }

    public class RabbitMQAuthorizedConsumer : RabbitMQConsumer
    {
        internal RabbitMQAuthorizedConsumer(RabbitMQSubscription subscription, AuthorizedMessageOperation operation, IRabbitMQMessageBus messageBus)
            : base(subscription, messageBus)
        {
            Operation = operation;
        }

        private readonly AuthorizedMessageOperation Operation;

        protected override void ExecuteSubscriptionMessageHandler(IBasicProperties properties, object message, Dictionary<string, string> headers)
        {
            var messageType = message.GetType();

            RabbitMQAuthorizer.Verify(Operation, messageType, message, headers);

            base.ExecuteSubscriptionMessageHandler(properties, message, headers);
        }
    }

    public class RabbitMQAuthorizedPublishingBus : RabbitMQPublishingBus, IAuthorizedPublishingBus
    {
        protected override void TryPublish(
            Type messageType, TopicId topicId, object message, StorageType storageType, 
            bool waitForPublishConfirmation, TimeSpan publishConfirmationTimeout, 
            Dictionary<string, string> headers, TimeSpan expiration)
        {
            RabbitMQAuthorizer.Verify(AuthorizedMessageOperation.Publish, messageType, message, headers);

            base.TryPublish(messageType, topicId, message, storageType, waitForPublishConfirmation, 
                publishConfirmationTimeout, headers, expiration);
        }
    }

    public class RabbitMQAuthorizedSubscriptionBus : RabbitMQSubscriptionBus, IAuthorizedSubscriptionBus
    {
        protected override RabbitMQSubscription NewSubscribeSubscription(Type messageType, SubscriptionId subscriptionId, TopicId topicId, SubscriptionMode subscriptionMode,
            bool acceptMessagesOlderThanSubscriptionTime, Action<object> messageHandler, string queueName)
        {
            return new RabbitMQAuthorizedSubscription(
                this, AuthorizedMessageOperation.Subscribe, subscriptionId, topicId, messageType, queueName,
                subscriptionMode, acceptMessagesOlderThanSubscriptionTime, messageHandler, null, null, null
            );
        }
    }

    public class RabbitMQAuthorizedRequestBus : RabbitMQRequestBus, IAuthorizedRequestBus
    {
        protected override RabbitMQSubscription NewResponseSubscription(Type responseType, string replyQueueName, Action<object, object> messageHandler)
        {
            return new RabbitMQAuthorizedSubscription(
                this, AuthorizedMessageOperation.Request, null, TopicId.None, responseType, replyQueueName,
                SubscriptionMode.Shared, true, null, messageHandler, null, null
            );
        }

        protected override void TryPublishRequest(
            Type requestType, IRequest request, string correlationId, string replyQueueName, 
            SubscriptionId subscriptionId, Dictionary<string, string> headers, TimeSpan expiration)
        {
            RabbitMQAuthorizer.Verify(AuthorizedMessageOperation.Request, requestType, request, headers);

            base.TryPublishRequest(requestType, request, correlationId, replyQueueName, subscriptionId, headers, expiration);
        }
    }

    public class RabbitMQAuthorizedResponseBus : RabbitMQResponseBus, IAuthorizedResponseBus
    {
        protected override RabbitMQSubscription NewRequestSubscription(Type requestType, string requestQueueName, Action<object, object> messageHandler)
        {
            return new RabbitMQAuthorizedSubscription(
                this, AuthorizedMessageOperation.Respond, null, TopicId.None, requestType, requestQueueName,
                SubscriptionMode.Shared, true, null, messageHandler, null, null
            );
        }

        protected override void TryPublishResponse(
            Type responseType, string replyQueueName, string correlationId, IResponse response, 
            Dictionary<string, string> headers, TimeSpan expiration)
        {
            RabbitMQAuthorizer.Verify(AuthorizedMessageOperation.Respond, responseType, response, headers);

            base.TryPublishResponse(responseType, replyQueueName, correlationId, response, headers, expiration);
        }
    }

    public class RabbitMQAuthorizedSendingBus : RabbitMQSendingBus, IAuthorizedSendingBus
    {
        protected override void TrySend(
            Type messageType, object message, SubscriptionId subscriptionId, StorageType storageType, 
            bool waitForPublishConfirmation, TimeSpan publishConfirmationTimeout,
            Dictionary<string, string> headers, TimeSpan expiration)
        {
            RabbitMQAuthorizer.Verify(AuthorizedMessageOperation.Send, messageType, message, headers);

            base.TrySend(messageType, message, subscriptionId, storageType, waitForPublishConfirmation, publishConfirmationTimeout, headers, expiration);
        }
    }

    public class RabbitMQAuthorizedReceivingBus : RabbitMQReceivingBus, IAuthorizedReceivingBus
    {
        protected override RabbitMQSubscription NewReceivingSubscription(Type messageType, SubscriptionId subscriptionId, SubscriptionMode subscriptionMode,
            bool acceptMessagesOlderThanSubscriptionTime, Action<object> messageHandler, string queueName)
        {
            return new RabbitMQAuthorizedSubscription(
                this, AuthorizedMessageOperation.Receive, subscriptionId, TopicId.None, messageType, queueName, 
                subscriptionMode, acceptMessagesOlderThanSubscriptionTime, messageHandler, null, null, null
            );
        }
    }
}
