using System;
using bisk.serdes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace bisk.MessageBus
{
    public class RabbitMqConsumer : IConsumer
    {
        private readonly string queueName;
        private readonly ISerDes serdes;
        private IConnection connection;
        private IModel channel;
        private readonly string RABBITMQ_HOST = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";

        public RabbitMqConsumer(string queueName, ISerDes serdes)
        {
            this.queueName = queueName;
            this.serdes = serdes;
        }

        public void Dispose()
        {
            if (channel != null) channel.Dispose();
            if (connection != null) connection.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Subscribe<TMessage>(Action<TMessage> handler) where TMessage : class
        {
            var factory = new ConnectionFactory() { HostName = RABBITMQ_HOST };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            // fair dispatch... (NOTE: queue can grow if consumer(s) is/are slow!
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var msg = serdes.Deserialize<TMessage>(ea.Body);
                handler(msg);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: false,    //true,
                                 consumer: consumer);
        }
    }
}