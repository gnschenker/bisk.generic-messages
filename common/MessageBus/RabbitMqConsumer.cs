using System;
using bisk.serdes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace bisk.MessageBus
{
    public class RabbitMqConsumer : IConsumer
    {
        private readonly ISerDes serdes;
        private IConnection connection;
        private IModel channel;
        private readonly string RABBITMQ_HOST = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        private readonly string QUEUE_NAME = Environment.GetEnvironmentVariable("QUEUE_NAME") ?? "bisk.sample.queue";

        public RabbitMqConsumer(ISerDes serdes)
        {
            Console.WriteLine("*** Using RabbitMQ Consumer");
            Console.WriteLine($"***** RabbitMQ Host: {RABBITMQ_HOST}");
            Console.WriteLine($"***** Queue Name:    {QUEUE_NAME}");

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
            channel.QueueDeclare(queue: QUEUE_NAME,
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
            channel.BasicConsume(queue: QUEUE_NAME,
                                 autoAck: false,    //true,
                                 consumer: consumer);
        }
    }
}