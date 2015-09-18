using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using PostSharp.Patterns.Diagnostics;
using System;

namespace ReactiveServices.MessageBus.RabbitMQ
{
    [Log(AttributeExclude = true)]
    [LogException(AttributeExclude = true)]
    internal class RabbitMQBsonSerializer : IMessageSerializer
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        private static readonly JsonSerializer JsonSerializer = JsonSerializer.Create(JsonSerializerSettings);

        public object Deserialize(byte[] messageBytes, Type messageType)
        {
            using (var stream = new MemoryStream(messageBytes))
            {
                using (var reader = new BsonReader(stream))
                {
                    return JsonSerializer.Deserialize(reader, messageType);
                }
            }
        }

        public byte[] Serialize(Type messageType, object message)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BsonWriter(stream))
                {
                    JsonSerializer.Serialize(writer, message, messageType);

                    var messageBytes = new byte[stream.Length];
                    stream.Position = 0;
                    stream.Read(messageBytes, 0, messageBytes.Length);
                    
                    return messageBytes;
                }
            }
        }
    }
}
