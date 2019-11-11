using System;
using bisk.serdes;
using RabbitMQ.Client;

namespace bisk.MessageBus
{
    public class RabbitMqPublisher : IPublisher
    {
        private readonly ISerDes serdes;
        private readonly IConnection connection;
        private readonly string RABBITMQ_HOST = 
            Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        private readonly string QUEUE_NAME =
            Environment.GetEnvironmentVariable("QUEUE_NAME") ?? "bisk.sample.queue";

        public IModel channel { get; }

        public RabbitMqPublisher(ISerDes serdes)
        {
            Console.WriteLine("*** Using RabbitMQ Publisher");
            Console.WriteLine($"***** RabbitMQ Host: {RABBITMQ_HOST}");
            Console.WriteLine($"***** Queue Name:    {QUEUE_NAME}");

            this.serdes = serdes;
            var factory = new ConnectionFactory() { HostName = RABBITMQ_HOST };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: QUEUE_NAME,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
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

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(exchange: "",
                                 routingKey: QUEUE_NAME,
                                 basicProperties: properties,
                                 body: body);
        }
    }
}