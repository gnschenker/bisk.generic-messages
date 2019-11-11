using System;
using bisk.serdes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace bisk.MessageBus
{
    public class RabbitMqDirectConsumer : IConsumer
    {
        private readonly ISerDes serdes;
        private IConnection connection;
        private IModel channel;
        private readonly string RABBITMQ_HOST = 
            Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        private readonly string EXCHANGE_NAME =
            Environment.GetEnvironmentVariable("EXCHANGE_NAME") ?? "bisk.sample.exchange";
        private readonly string ROUTING_KEY =
            Environment.GetEnvironmentVariable("ROUTING_KEY") ?? "bisk.messages.GenericMessage";

        public RabbitMqDirectConsumer(ISerDes serdes)
        {
            Console.WriteLine("*** Using RabbitMQ Direct Consumer");
            Console.WriteLine($"***** RabbitMQ Host: {RABBITMQ_HOST}");
            Console.WriteLine($"***** Exchange Name: {EXCHANGE_NAME}");
            Console.WriteLine($"***** Routing Key:   {ROUTING_KEY}");

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
            channel.ExchangeDeclare(exchange: EXCHANGE_NAME, type: "direct");
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: EXCHANGE_NAME,
                              routingKey: ROUTING_KEY);


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