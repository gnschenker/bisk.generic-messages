using System;
using bisk.serdes;

namespace bisk.messages
{
    public class GenericMessage
    {
        public class MessageBody{
            public Guid CorrelationId { get; set; }
            public string Body { get; set; }
            public DateTime ResponseTimeStamp { get; set; }
            public string Topic { get; set; }
        }

        public class MessageEvent
        {
            public bool Replay { get; set; }
            public bool Rollback { get; set; }
            public bool NormalOperation { get; set; }
        }

        public class MessageStatus
        {
            public bool Success { get; set; }
            public string Error { get; set; }
        }

        public MessageBody Message { get; set; }
        public MessageEvent Event { get; set; }
        public MessageStatus Status { get; set; }

        public void SetPayload<TMessage>(TMessage payload) where TMessage: class
        {
            Message.Body = new JsonSerDes().Stringify(payload);
        }

        public TMessage GetPayload<TMessage>() where TMessage: class
        {
            return new JsonSerDes().FromString<TMessage>(Message.Body);
        }
    }
}