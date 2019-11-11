using System;
using bisk.serdes;
using RabbitMQ.Client;

namespace bisk.MessageBus
{
    public class RabbitMqDirectPublisher : IPublisher
    {
        private readonly ISerDes serdes;
        private readonly IConnection connection;
        private readonly IModel channel;
        
        private readonly string RABBITMQ_HOST = 
            Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        private readonly string EXCHANGE_NAME =
            Environment.GetEnvironmentVariable("EXCHANGE_NAME") ?? "bisk.sample.exchange";


        public RabbitMqDirectPublisher(ISerDes serdes)
        {
            Console.WriteLine("*** Using RabbitMQ Direct Publisher ***");
            Console.WriteLine($"***** RabbitMQ Host: {RABBITMQ_HOST}");
            Console.WriteLine($"***** Exchange Name: {EXCHANGE_NAME}");

            this.serdes = serdes;
            var factory = new ConnectionFactory() { HostName = RABBITMQ_HOST };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: EXCHANGE_NAME, type: "direct");
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
            channel.BasicPublish(exchange: EXCHANGE_NAME,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
        }
    }
}