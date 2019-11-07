namespace publisher
{
    public interface ISerDes
    {
        TMessage Deserialize<TMessage>(byte[] body) where TMessage : class;
        byte[] Serialize<TMessage>(TMessage message) where TMessage: class;
    }
}