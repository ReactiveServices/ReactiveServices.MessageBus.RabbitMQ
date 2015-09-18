using System;
using System.Runtime.Serialization;

namespace ReactiveServices.MessageBus.Tests
{
    [DataContract]
    public class AMessage : Message
    {
    }

    [DataContract]
    public class EventOccurred : Message
    {
        [DataMember]
        public string RelevantField1 { get; set; }
        [DataMember]
        public string RelevantField2 { get; set; }
    }

    [DataContract]
    public class OtherEventOccurred : Message
    {
    }

    [DataContract]
    public class RequestX : Request
    {
        public RequestX(RequesterId requesterId, RequestId requestId) : base(requesterId, requestId)
        {
        }
    }

    [DataContract]
    public class RequestY : Request
    {
        public RequestY(RequesterId requesterId, RequestId requestId) : base(requesterId, requestId)
        {
        }
    }

    [DataContract]
    public class ResponseX : Response
    {
        public ResponseX(ResponderId responderId, ResponseId responseId, IRequest request) : base(responderId, responseId, request)
        {
        }
    }

    [DataContract]
    public class ResponseY : Response
    {
        public ResponseY(ResponderId responderId, ResponseId responseId, IRequest request) : base(responderId, responseId, request)
        {
        }
    }
}
