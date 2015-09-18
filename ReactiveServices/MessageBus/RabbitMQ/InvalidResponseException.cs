using System;
using PostSharp.Patterns.Diagnostics;

namespace ReactiveServices.MessageBus.RabbitMQ
{
    [Log(AttributeExclude = true)]
    [LogException(AttributeExclude = true)]
    public class InvalidCorrelationIdException : Exception
    {
        public InvalidCorrelationIdException(string requestTypeName, string correlationId)
            : base(String.Format("Invalid correlation id '{0}' for request of type '{1}'.", correlationId, requestTypeName))
        {
        }
    }
}
