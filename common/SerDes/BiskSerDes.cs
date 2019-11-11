using bisk.messages;

namespace bisk.serdes
{
    public class BiskSerDes
    {
        private readonly JsonSerDes jsonSerdes = new JsonSerDes();

        public string Stringify<TMessage>(GenericMessage wrapperMessage, TMessage payload) where TMessage: class
        {
            var jsonPayload = jsonSerdes.Stringify(payload);
            wrapperMessage.Message.Body = jsonPayload;
            return jsonSerdes.Stringify(wrapperMessage);
        }

        public byte[] Serialize<TMessage>(GenericMessage wrapperMessage, TMessage payload) where TMessage: class
        {
            var jsonPayload = jsonSerdes.Stringify(payload);
            wrapperMessage.Message.Body = jsonPayload;
            return jsonSerdes.Serialize(wrapperMessage);
        }

        public DeserializationResult Deserialize<TMessage>(byte[] body) where TMessage: class
        {
            var msg = jsonSerdes.Deserialize<GenericMessage>(body);
            var payload = jsonSerdes.FromString<TMessage>(msg.Message.Body);
            return new DeserializationResult
            {
                WrapperMessage = msg,
                Payload = payload
            };
        }

        public class DeserializationResult
        {
            public GenericMessage WrapperMessage { get; internal set; }
            public object Payload { get; internal set; }
        }
    }
}
