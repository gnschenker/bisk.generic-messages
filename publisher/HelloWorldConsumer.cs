using System;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace publisher
{
    public class HelloWorldConsumer : IConsumer
    {
        private readonly string queueName;
        private readonly ISerDes serdes;
        private readonly IConnection connection;
        private IModel channel;

        public HelloWorldConsumer(string queueName, ISerDes serdes)
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

        public void Subscribe<TMessage>(WaitHandle waitHandle, Action<TMessage> handler) where TMessage : class
        {
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var msg = serdes.Deserialize<TMessage>(ea.Body);
                handler(msg);
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            waitHandle.WaitOne();
        }
    }
}