using System;
using System.Threading;
using RabbitMQ.Client;

namespace publisher
{
    public class HelloWorldPublisher : IPublisher
    {
        private readonly string queueName;
        private readonly ISerDes serdes;
        private readonly IConnection connection;

        public IModel channel { get; }

        public HelloWorldPublisher(string queueName, ISerDes serdes)
        {
            this.queueName = queueName;
            this.serdes = serdes;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void Dispose()
        {
            if (channel != null) channel.Dispose();
            if (connection != null) connection.Dispose();
        }

        public void Publish<TMessage>(TMessage message) where TMessage : class
        {
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = serdes.Serialize(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine(" [x] Sent {0}", message);
            Thread.Sleep(500);
        }
    }
}