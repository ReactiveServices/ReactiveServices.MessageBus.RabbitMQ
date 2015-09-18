using System.Diagnostics;
using NLog;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PostSharp.Patterns.Diagnostics;

namespace ReactiveServices.MessageBus.RabbitMQ
{
    public class RabbitMQRequestBus : RabbitMQMessageBus, IRequestBus
    {
        public RabbitMQRequestBus()
        {
            RequestTimeout = TimeSpan.FromSeconds(30);
        }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly List<string> ReplyQueueNames = new List<string>();
        private readonly Queue<RabbitMQSubscription> RepliesSubscriptions = new Queue<RabbitMQSubscription>();

        protected TimeSpan RequestTimeout { get; set; }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public TResponse Request<TRequest, TResponse>(
            TRequest request, 
            SubscriptionId subscriptionId = null,
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan))
            where TRequest : class, IRequest
            where TResponse : class, IResponse
        {
            if (request == null) throw new ArgumentNullException("request");

            return (TResponse)TryRequest(typeof(TRequest), typeof(TResponse), request, subscriptionId, headers, expiration);
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public IResponse Request(
            Type requestType, Type responseType, IRequest request, 
            SubscriptionId subscriptionId = null,
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan))
        {
            if (request == null) throw new ArgumentNullException("request");
            if (requestType == null) throw new ArgumentNullException("requestType");
            if (responseType == null) throw new ArgumentNullException("responseType");

            return TryRequest(requestType, responseType, request, subscriptionId, headers, expiration);
        }

        [LogException]
        private IResponse TryRequest(
            Type requestType, Type responseType, IRequest request, 
            SubscriptionId subscriptionId,
            Dictionary<string, string> headers,
            TimeSpan expiration)
        {
            var correlationId = Guid.NewGuid().ToString();
            var replyQueueName = ReplyQueueNameFor(requestType, correlationId);
            IResponse response = null;

            var subscription = NewResponseSubscription(responseType, replyQueueName, (message, props) =>
            {
                if (((IBasicProperties)props).CorrelationId != correlationId)
                    throw new InvalidCorrelationIdException(requestType.Name, correlationId);

                response = (IResponse)message;
            });

            subscription.Start();
            RepliesSubscriptions.Enqueue(subscription);

            TryPublishRequest(requestType, request, correlationId, replyQueueName, subscriptionId, headers, expiration);

            var requestProcessingTime = new Stopwatch();
            requestProcessingTime.Start();
            while (response == null && requestProcessingTime.Elapsed < RequestTimeout)
            {
                Thread.Sleep(10);
            }
            requestProcessingTime.Stop();

            if (response == null)
            {
                throw new TimeoutException(
                    String.Format(
                        "Could not receive a response for the request of correlation id {0} within {1} milliseconds",
                        correlationId,
                        RequestTimeout.TotalMilliseconds
                        )
                    );
            }
            return response;
        }

        protected virtual RabbitMQSubscription NewResponseSubscription(
            Type responseType, string replyQueueName, 
            Action<object, object> messageHandler)
        {
            var subscription = new RabbitMQSubscription(
                this, null, TopicId.None, responseType, replyQueueName, SubscriptionMode.Shared, 
                true, null, messageHandler, null, null);
            return subscription;
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public async Task<TResponse> RequestAsync<TRequest, TResponse>(
            TRequest request, 
            SubscriptionId subscriptionId = null,
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan))
            where TRequest : class, IRequest
            where TResponse : class, IResponse
        {
            if (request == null) throw new ArgumentNullException("request");

            var response = await TryRequestAsync(typeof(TRequest), typeof(TResponse), request, subscriptionId, headers, expiration);
            return (TResponse)response;
        }

        [Log(AttributeExclude = true)]
        [LogException(AttributeExclude = true)]
        public async Task<IResponse> RequestAsync(
            Type requestType, Type responseType, IRequest request, 
            SubscriptionId subscriptionId = null,
            Dictionary<string, string> headers = null,
            TimeSpan expiration = default(TimeSpan))
        {
            if (request == null) throw new ArgumentNullException("request");
            if (requestType == null) throw new ArgumentNullException("requestType");
            if (responseType == null) throw new ArgumentNullException("responseType");

            return await TryRequestAsync(requestType, responseType, request, subscriptionId, headers, expiration);
        }

        [LogException]
        private async Task<IResponse> TryRequestAsync(
            Type requestType, Type responseType, IRequest request, SubscriptionId subscriptionId, 
            Dictionary<string, string> headers, TimeSpan expiration)
        {
            var correlationId = Guid.NewGuid().ToString();
            var replyQueueName = ReplyQueueNameFor(requestType, correlationId);
            IResponse response = null;
            var subscription = new RabbitMQSubscription(
                this, null, TopicId.None, responseType, replyQueueName, SubscriptionMode.Shared, true, null, (message, props) =>
            {
                if (((IBasicProperties)props).CorrelationId != correlationId)
                    throw new InvalidCorrelationIdException(requestType.Name, correlationId);

                response = (IResponse)message;
            }, null, null);
            subscription.Start();
            RepliesSubscriptions.Enqueue(subscription);
            return await Task.Run(() =>
            {
                TryPublishRequest(requestType, request, correlationId, replyQueueName, subscriptionId, headers, expiration);

                var requestProcessingTime = new Stopwatch();
                requestProcessingTime.Start();
                while (response == null && requestProcessingTime.Elapsed < RequestTimeout)
                {
                    Thread.Sleep(10);
                }
                requestProcessingTime.Stop();

                if (response == null)
                {
                    throw new TimeoutException(
                        String.Format(
                            "Could not receive a response for the request of correlation id {0} within {1} milliseconds",
                            correlationId,
                            RequestTimeout.TotalMilliseconds
                        )
                    );
                }
                return response;
            });
        }

        private string ReplyQueueNameFor(Type requestType, string correlationId)
        {
            var subscriptionId = SubscriptionId.FromString(String.Format("ReplyFromCorrelationId_{0}", correlationId));
            var replyQueueName = QueueNameFor(requestType, subscriptionId);
            ReplyQueueNames.Add(replyQueueName);
            return replyQueueName;
        }

        [LogException]
        protected virtual void TryPublishRequest(
            Type requestType, IRequest request, string correlationId, 
            string replyQueueName, SubscriptionId subscriptionId,
            Dictionary<string, string> headers,
            TimeSpan expiration)
        {
            try
            {
                var requestQueueName = subscriptionId == null
                    ? QueueNameFor(requestType)
                    : QueueNameFor(requestType, subscriptionId);
                var requestMessageBody = Serializer.Serialize(requestType, request);

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

                    props.ReplyTo = replyQueueName;
                    props.CorrelationId = correlationId;
                    
                    model.BasicPublish("", requestQueueName, props, requestMessageBody);
                }
                Log.Info("Request with correlation id '{0}' published to queue '{1}' waiting reply on queue '{2}'", correlationId, requestQueueName, replyQueueName);
                Log.Debug("Request with correlation id '{0}': '{1}'", correlationId, Encoding.UTF8.GetString(requestMessageBody));
            }
            catch (Exception e)
            {
                Log.Error(e, "Could not publish request of type '{0}'", requestType.Name);
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeRepliesSubscriptions();
                DeleteReplyQueues();
            }
        }

        private void DeleteReplyQueues()
        {
            using (var model = NewChannel())
            {
                foreach (var replyQueueName in ReplyQueueNames)
                {
                    model.QueueDelete(replyQueueName);
                }
            }
        }

        private void DisposeRepliesSubscriptions()
        {
            foreach (var subscription in RepliesSubscriptions.ToArray())
            {
                subscription.Dispose();
            }
        }

        public void DeleteRequestQueue(Type requestType, SubscriptionId subscriptionId = null)
        {
            var requestQueueName = subscriptionId == null
                ? QueueNameFor(requestType)
                : QueueNameFor(requestType, subscriptionId);

            using (var model = NewChannel())
            {
                model.QueueDelete(requestQueueName);
            }
        }
    }
}
