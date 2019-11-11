using System;
using bisk.serdes;
using RabbitMQ.Client;

namespace bisk.MessageBus
{
    public class RabbitMqDirectPublisher : IPublisher
    {
        private readonly string exchangeName;
        private readonly ISerDes serdes;
        private readonly IConnection connection;
        private readonly string RABBITMQ_HOST = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        
        public IModel channel { get; }

        public RabbitMqDirectPublisher(string exchangeName, ISerDes serdes)
        {
            this.exchangeName = exchangeName;
            this.serdes = serdes;
            var factory = new ConnectionFactory() { HostName = RABBITMQ_HOST };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: exchangeName, type: "direct");
        }

        public void Dispose()
        {
            if (channel != null) channel.Dispose();
            if (connection != null) connection.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Publish<TMessage>(TMessage message) where TMessage : class
        {
            var body = serdes.Serialize(message);

            var routingKey = message.GetType().FullName;
            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
        }
    }
}