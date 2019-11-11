using System;
using bisk.serdes;
using RabbitMQ.Client;

namespace bisk.MessageBus
{
    public class RabbitMqPublisher : IPublisher
    {
        private readonly string queueName;
        private readonly ISerDes serdes;
        private readonly IConnection connection;
        private readonly string RABBITMQ_HOST = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";

        public IModel channel { get; }

        public RabbitMqPublisher(string queueName, ISerDes serdes)
        {
            Console.WriteLine("*** Using RabbitMQ Publisher");
            this.queueName = queueName;
            this.serdes = serdes;
            var factory = new ConnectionFactory() { HostName = RABBITMQ_HOST };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName,
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
                                 routingKey: queueName,
                                 basicProperties: properties,
                                 body: body);
        }
    }
}