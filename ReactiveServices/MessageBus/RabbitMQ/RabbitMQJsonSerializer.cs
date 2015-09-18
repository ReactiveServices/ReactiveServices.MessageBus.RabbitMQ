using Newtonsoft.Json;
using System;
using System.Text;
using NLog;
using PostSharp.Patterns.Diagnostics;

namespace ReactiveServices.MessageBus.RabbitMQ
{
    [Log(AttributeExclude = true)]
    [LogException(AttributeExclude = true)]
    internal class RabbitMQJsonSerializer : IMessageSerializer
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            DateTimeZoneHandling = DateTimeZoneHandling.Local
        };

        public object Deserialize(byte[] messageBytes, Type messageType)
        {
            var messageText = Encoding.UTF8.GetString(messageBytes);
            var messageObject = JsonConvert.DeserializeObject(messageText, messageType, JsonSerializerSettings);
            return messageObject;
        }

        public byte[] Serialize(Type messageType, object message)
        {
            var messageText = JsonConvert.SerializeObject(message, messageType, JsonSerializerSettings);

            Log.Debug("Serialized Message: [{0}] {1}", messageType.Name, messageText);

            var messageBytes = Encoding.UTF8.GetBytes(messageText);
            return messageBytes;
        }
    }
}
