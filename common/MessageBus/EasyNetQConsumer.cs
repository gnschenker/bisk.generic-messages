using EasyNetQ;
using EasyNetQ.Topology;
using System;

namespace bisk.MessageBus
{
    public class EasyNetQConsumer : IConsumer
    {
        private readonly string RABBITMQ_HOST = 
            Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        private readonly string queueName;
        private readonly IAdvancedBus advancedBus;

        public EasyNetQConsumer(string queueName)
        {
            Console.WriteLine("*** Using EasyNetQ Consumer");
            this.queueName = queueName;
            advancedBus = RabbitHutch.CreateBus($"host={RABBITMQ_HOST}").Advanced;
        }

        public void Dispose()
        {
        }

        public void Subscribe<TMessage>(Action<TMessage> handler) where TMessage : class
        {
            var queue = advancedBus.QueueDeclare(queueName,
                                                 durable: true,
                                                 exclusive: false,
                                                 autoDelete: false);

            advancedBus.Consume(queue, x => x
                .Add<TMessage>((message, info) => handler(message.Body)));
        }
    }
}
