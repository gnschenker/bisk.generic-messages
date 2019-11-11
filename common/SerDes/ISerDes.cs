namespace bisk.serdes
{
    public interface ISerDes
    {
        TMessage Deserialize<TMessage>(byte[] body) where TMessage : class;
        TMessage FromString<TMessage>(string value) where TMessage : class;
        byte[] Serialize<TMessage>(TMessage message) where TMessage: class;
        string Stringify<TMessage>(TMessage message) where TMessage: class;
    }
}