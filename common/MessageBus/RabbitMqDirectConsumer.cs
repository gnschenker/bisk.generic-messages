using System;
using bisk.serdes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace bisk.MessageBus
{
    public class RabbitMqDirectConsumer : IConsumer
    {
        private readonly string exchangeName;
        private readonly string routingKey;
        private readonly ISerDes serdes;
        private IConnection connection;
        private IModel channel;
        private readonly string RABBITMQ_HOST = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";

        public RabbitMqDirectConsumer(string exchangeName, string routingKey, ISerDes serdes)
        {
            this.exchangeName = exchangeName;
            this.serdes = serdes;
            this.routingKey = routingKey;
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
            channel.ExchangeDeclare(exchange: exchangeName,
                                    type: "direct");
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                                  exchange: exchangeName,
                                  routingKey: routingKey);


            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var msg = serdes.Deserialize<TMessage>(ea.Body);
                handler(msg);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: exchangeName,
                                 autoAck: false,    //true,
                                 consumer: consumer);
        }
    }
}