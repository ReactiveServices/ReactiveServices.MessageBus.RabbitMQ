using MessageBus.RabbitMQ.ConnectionString;
using Newtonsoft.Json;
using NLog;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using ReactiveServices.Configuration;
using ReactiveServices.Configuration.ConfigurationFiles;
using ReactiveServices.Extensions;

namespace ReactiveServices.MessageBus.RabbitMQ
{
    public interface IRabbitMQMessageBus
    {
        IModel NewChannel();
        bool QueueExists(string queueName);
    }

    public abstract class RabbitMQMessageBus : IMessageBus, IRabbitMQMessageBus
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly ConnectionStringParser ConnectionStringParser = new ConnectionStringParser();

        private ConnectionFactory _connectionFactory;
        private ConnectionFactory ConnectionFactory
        {
            get
            {
                if (_connectionFactory == null)
                {
                    var connectionConfiguration = ConnectionStringParser.Parse(Settings.ConnectionStrings.MessageBus.ConnectionString);

                    var hostConfiguration = connectionConfiguration.Hosts.First();

                    _connectionFactory = new ConnectionFactory();

                    if (connectionConfiguration.AMQPConnectionString != null)
                    {
                        _connectionFactory.uri = connectionConfiguration.AMQPConnectionString;
                    }

                    _connectionFactory.HostName = hostConfiguration.Host;
                    _connectionFactory.VirtualHost = connectionConfiguration.VirtualHost;
                    _connectionFactory.UserName = connectionConfiguration.UserName;
                    _connectionFactory.Password = connectionConfiguration.Password;
                    _connectionFactory.Port = hostConfiguration.Port;
                    _connectionFactory.Ssl = connectionConfiguration.Ssl;
                    _connectionFactory.RequestedHeartbeat = connectionConfiguration.RequestedHeartbeat;
                    _connectionFactory.ClientProperties = connectionConfiguration.ClientProperties;
                }

                return _connectionFactory;
            }
        }

        public IModel NewChannel()
        {
#if DEBUG
            ConnectionFactory.UpdateStackTraceOnClientProperties();
#endif
            var model = new RabbitMQChannel(ConnectionFactory);
            return model;
        }

        protected static readonly IMessageSerializer Serializer = DependencyResolver.Get<IMessageSerializer>();

        protected static string QueueNameFor(Type messageType)
        {
            //Ex: ReactiveServices.ComputationalUnit.Dispatching.LaunchConfirmation:ComputationalUnit.Dispatching
            //Pattern: FullTypeName:AssemblyName_SubscriptionId
            return String.Format("{0}:{1}", messageType.FullName, messageType.Assembly.GetName().Name);
        }

        protected static string QueueNameFor(Type messageType, SubscriptionId subscriptionId)
        {
            //Ex: ReactiveServices.ComputationalUnit.Dispatching.LaunchConfirmation:ComputationalUnit.Dispatching_LaunchConfirmationSubscriptionFor_DispatcherLauncherId#c73c2193-a8c5-47b6-ac5a-cde91a4c788b‏
            //Pattern: FullTypeName:AssemblyName_SubscriptionId
            return String.Format("{0}:{1}_{2}", messageType.FullName, messageType.Assembly.GetName().Name, subscriptionId.Value);
        }

        protected static string ExchangeNameFor(Type messageType)
        {
            //Ex: ReactiveServices.ComputationalUnit.Dispatching.LaunchConfirmation:ComputationalUnit.Dispatching‏
            //Pattern: FullTypeName:AssemblyName
            return String.Format("{0}:{1}", messageType.FullName, messageType.Assembly.GetName().Name);
        }

        protected static void BindQueue(IModel model, Subscription subscription)
        {
            var queueName = QueueNameFor(subscription.MessageType, subscription.SubscriptionId);
            var exchangeName = ExchangeNameFor(subscription.MessageType);
            var routingKey = RoutingKeyFor(subscription.TopicId);
            model.QueueBind(queueName, exchangeName, routingKey);
            Log.Info("Queue '{0}' bound to exchange {1}!", queueName, exchangeName);
        }

        protected static string RoutingKeyFor(TopicId topicId)
        {
            var routingKey = topicId == TopicId.Default ? "#" : topicId.Value;
            return routingKey;
        }

        protected bool ExchangeExists(string exchangeName)
        {
            try
            {
                var client = new WebClient();

                var uri = PrepareMaintenanceCommand(client, "exchanges", exchangeName);

                var responseText = client.DownloadString(uri);

                dynamic queue = JsonConvert.DeserializeObject(responseText);

                var name = (string)queue.name.Value;

                return (name == exchangeName);
            }
            catch
            {
                return false;
            }
        }

        protected void DeleteExchange(string exchangeName)
        {
            try
            {
                var client = new WebClient();

                var uri = PrepareMaintenanceCommand(client, "exchanges", exchangeName);

                client.UploadString(uri, "DELETE", String.Empty);
            }
            catch (WebException we)
            {
                Log.Info("Web Exception trying to delete exchange {0}. Status: {1}!", exchangeName, we.Status);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error trying to exchange queue {0}!", exchangeName);
            }
        }

        protected static void DeclareExchange(IModel model, Subscription subscription)
        {
            var exchangeName = ExchangeNameFor(subscription.MessageType);
            model.ExchangeDeclare(exchangeName, "topic");
            Log.Info("Exchange '{0}' declared!", exchangeName);
        }

        protected static void DeclareQueue(IModel model, Subscription subscription)
        {
            var queueName = QueueNameFor(subscription.MessageType, subscription.SubscriptionId);
            var isExclusive = subscription.SubscriptionMode == SubscriptionMode.Exclusive;
            DeclareQueue(model, queueName, isExclusive);
        }

        internal static void DeclareQueue(IModel model, string queueName, bool isExclusive)
        {
            model.QueueDeclare(queueName, true, isExclusive, false, new Dictionary<string, object>());
            Log.Info("Queue '{0}' declared!", queueName);
        }

        protected void DeclareQueue(string queueName)
        {
            using (var model = NewChannel())
            {
                DeclareQueue(model, queueName, false);
            }
        }

        public bool QueueExists(string queueName)
        {
            try
            {
                var client = new WebClient();

                var uri = PrepareMaintenanceCommand(client, "queues", queueName);

                var responseText = client.DownloadString(uri);

                dynamic queue = JsonConvert.DeserializeObject(responseText);

                var name = (string)queue.name.Value;

                return (name == queueName);
            }
            catch
            {
                return false;
            }
        }

        protected void DeleteQueue(string queueName)
        {
            try
            {
                var client = new WebClient();

                var uri = PrepareMaintenanceCommand(client, "queues", queueName);

                client.UploadString(uri, "DELETE", String.Empty);
            }
            catch (WebException we)
            {
                Log.Info("Web Exception trying to delete queue {0}. Status: {1}!", queueName, we.Status);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error trying to delete queue {0}!", queueName);
            }
        }

        protected Uri PrepareMaintenanceCommand(WebClient client, string resourceType, string resourceName = null)
        {
            var hostUrl = ConnectionFactory.HostName;
            const int portNumber = 15672; //TODO: Make configurable
            var username = ConnectionFactory.UserName;
            var password = ConnectionFactory.Password;
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(String.Format("{0}:{1}", username, password)));
            var uriValue = String.Format("http://{0}:{1}/api/{2}", hostUrl, portNumber, resourceType);
            if (!String.IsNullOrWhiteSpace(resourceName))
                uriValue = String.Format("{0}/%2f/{1}", uriValue, resourceName);
            var uri = new Uri(uriValue);
            client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);
            return uri;
        }

        protected void SetMessageExpirationTimespan(IBasicProperties props, TimeSpan expiration)
        {
            // Once setted in props, expiration will be hanbled by RabbitMQ
            var timestamp = new AmqpTimestamp(DateTime.Now.ToUnixTime());
            props.Timestamp = timestamp;
            if (expiration != default(TimeSpan))
                props.Expiration = expiration.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
