using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using RabbitMQ.Client;
using System;

namespace ReactiveServices.MessageBus.RabbitMQ
{
    public class RabbitMQResponseBus : RabbitMQMessageBus, IResponseBus
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<Tuple<Type, SubscriptionId>, Tuple<RabbitMQSubscription, CancellationTokenSource>> ActiveSubscriptions = new Dictionary<Tuple<Type, SubscriptionId>, Tuple<RabbitMQSubscription, CancellationTokenSource>>();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var requestType in ActiveSubscriptions.Keys.ToArray())
                {
                    StopRespondingTo(requestType.Item1, requestType.Item2);
                }
            }
        }

        public void StartRespondingTo<TRequest, TResponse>(
            Func<TRequest, TResponse> requestReceivedCallback, 
            SubscriptionId subscriptionId = null,
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan))
            where TRequest : class, IRequest
            where TResponse : class, IResponse
        {
            if (requestReceivedCallback == null)
                throw new ArgumentNullException("requestReceivedCallback");

            TryRespondTo(typeof(TRequest), typeof(TResponse), r => requestReceivedCallback((TRequest)r), subscriptionId, headers, expiration);
        }

        public void StartRespondingTo(
            Type requestType, Type responseType, 
            Func<IRequest, IResponse> requestReceivedCallback, 
            SubscriptionId subscriptionId = null,
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan))
        {
            if (requestType == null)
                throw new ArgumentNullException("requestType");

            if (responseType == null)
                throw new ArgumentNullException("responseType");

            if (requestReceivedCallback == null)
                throw new ArgumentNullException("requestReceivedCallback");

            TryRespondTo(requestType, responseType, requestReceivedCallback, subscriptionId, headers, expiration);
        }

        public void TryRespondTo(
            Type requestType, Type responseType, 
            Func<IRequest, IResponse> requestReceivedCallback, 
            SubscriptionId subscriptionId,
            Dictionary<string, string> headers,
            TimeSpan expiration)
        {
            var requestQueueName = subscriptionId == null ? QueueNameFor(requestType) : QueueNameFor(requestType, subscriptionId);
            IRequest request = null;
            IBasicProperties properties = null;

            var subscription = NewRequestSubscription(requestType, requestQueueName, (message, props) =>
            {
                properties = (IBasicProperties)props;

                if (String.IsNullOrEmpty(properties.CorrelationId))
                    throw new InvalidCorrelationIdException(requestType.Name, properties.CorrelationId);

                request = (IRequest)message;
            });

            var activeSubscriptionKey = new Tuple<Type, SubscriptionId>(requestType, subscriptionId);
            if (ActiveSubscriptions.ContainsKey(activeSubscriptionKey))
                throw new InvalidOperationException(String.Format("There is already a subscription of id {0} for type {1}", subscriptionId, requestType));

            subscription.Start();
            var respondingTaskCancellationTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                while (!respondingTaskCancellationTokenSource.IsCancellationRequested) // Blocks until a request is received
                {
                    if (request != null)
                    {
                        //Note: Expired requests will be handled, and ignored, by RabbitMQ itself
                        var response = requestReceivedCallback(request);
                        var responseHeaders = new Dictionary<string, string>();
                        if (headers != null)
                        {
                            foreach (var header in headers)
                            {
                                responseHeaders.Add(header.Key, header.Value);
                            }
                        }

                        Task.Run(
                            () => TryPublishResponse(
                                responseType,
                                properties.ReplyTo,
                                properties.CorrelationId,
                                response, 
                                responseHeaders,
                                expiration
                            ), 
                            respondingTaskCancellationTokenSource.Token
                        );

                        // Continues waiting for other requests, but nullify the last one to avoid respond it again
                        request = null;
                    }
                    Thread.Sleep(10);
                }
            }, respondingTaskCancellationTokenSource.Token);

            var key = new Tuple<Type, SubscriptionId>(requestType, subscriptionId);
            ActiveSubscriptions[key] = new Tuple<RabbitMQSubscription, CancellationTokenSource>(subscription, respondingTaskCancellationTokenSource);
        }

        protected virtual RabbitMQSubscription NewRequestSubscription(Type requestType, string requestQueueName, Action<object, object> messageHandler)
        {
            var subscription = new RabbitMQSubscription(this, null, TopicId.None, requestType, requestQueueName, SubscriptionMode.Shared, true, null, messageHandler, null, null);
            return subscription;
        }

        protected virtual void TryPublishResponse(
            Type responseType, string replyQueueName, string correlationId, 
            IResponse response, Dictionary<string, string> headers,
            TimeSpan expiration)
        {
            try
            {
                var responseMessageBody = Serializer.Serialize(responseType, response);
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

                    props.CorrelationId = correlationId;

                    model.BasicPublish("", replyQueueName, props, responseMessageBody);

                    Log.Info("Response for request with correlation id '{0}' published to queue '{1}'", props.CorrelationId, replyQueueName);
                    Log.Debug("Response for request with correlation id '{0}': '{1}'", props.CorrelationId, Encoding.UTF8.GetString(responseMessageBody));
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Could not publish response of type '{0}'", responseType.Name);
                throw;
            }
        }

        public void StopRespondingTo<TRequest>(SubscriptionId subscriptionId = null) where TRequest : class, IRequest
        {
            StopRespondingTo(typeof(TRequest), subscriptionId);
        }

        public void StopRespondingTo(Type requestType, SubscriptionId subscriptionId = null)
        {
            if (requestType == null)
                throw new ArgumentNullException("requestType");

            Tuple<RabbitMQSubscription, CancellationTokenSource> subscriptionAndCancellationTokenSource;

            var key = new Tuple<Type, SubscriptionId>(requestType, subscriptionId);

            if (ActiveSubscriptions.TryGetValue(key, out subscriptionAndCancellationTokenSource))
            {
                var respondingTaskCancellationTokenSource = subscriptionAndCancellationTokenSource.Item2;
                respondingTaskCancellationTokenSource.Cancel();
                var subscription = subscriptionAndCancellationTokenSource.Item1;
                subscription.Dispose();
                ActiveSubscriptions.Remove(key);
            }
        }
    }
}
