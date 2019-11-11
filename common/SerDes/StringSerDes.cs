using System.Text;

namespace bisk.serdes
{
    public class StringSerDes
    {
        public string Deserialize(byte[] body)
        {
            return Encoding.UTF8.GetString(body);
        }

        public byte[] Serialize(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }
    }
}
