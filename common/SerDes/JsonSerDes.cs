using Newtonsoft.Json;
using System.Text;

namespace bisk.serdes
{
    public class JsonSerDes : ISerDes
    {
        public TMessage Deserialize<TMessage>(byte[] body) where TMessage : class
        {
            var messageJson = Encoding.UTF8.GetString(body);
            return FromString<TMessage>(messageJson);
        }

        public TMessage FromString<TMessage>(string value) where TMessage : class
        {
            return JsonConvert.DeserializeObject<TMessage>(value);
        }

        public byte[] Serialize<TMessage>(TMessage message) where TMessage : class
        {
            string messageJson = Stringify(message);
            return Encoding.UTF8.GetBytes(messageJson);
        }

        public string Stringify<TMessage>(TMessage message) where TMessage: class
        {
            return JsonConvert.SerializeObject(message);
        }
    }
}
