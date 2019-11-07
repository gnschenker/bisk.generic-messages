using Newtonsoft.Json;
using System.Text;

namespace publisher
{
    public class JsonSerDes : ISerDes
    {
        public TMessage Deserialize<TMessage>(byte[] body) where TMessage : class
        {
            var messageJson = Encoding.UTF8.GetString(body);
            var message = JsonConvert.DeserializeObject<TMessage>(messageJson);
            return message;
        }

        public byte[] Serialize<TMessage>(TMessage message) where TMessage : class
        {
            string messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);
            return body;
        }
    }
}
