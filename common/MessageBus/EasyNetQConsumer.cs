using EasyNetQ;
using EasyNetQ.Topology;
using System;

namespace bisk.MessageBus
{
    public class EasyNetQConsumer : IConsumer
    {
        private readonly string RABBITMQ_HOST = 
            Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        private readonly string QUEUE_NAME = 
            Environment.GetEnvironmentVariable("QUEUE_NAME") ?? "bisk.sample.queue";
        private readonly IAdvancedBus advancedBus;

        public EasyNetQConsumer()
        {
            Console.WriteLine("*** Using EasyNetQ Consumer");
            Console.WriteLine($"***** RabbitMQ Host: {RABBITMQ_HOST}");
            Console.WriteLine($"***** Queue Name:    {QUEUE_NAME}");

            advancedBus = RabbitHutch.CreateBus($"host={RABBITMQ_HOST}").Advanced;
        }

        public void Dispose()
        {
        }

        public void Subscribe<TMessage>(Action<TMessage> handler) where TMessage : class
        {
            var queue = advancedBus.QueueDeclare(QUEUE_NAME,
                                                 durable: true,
                                                 exclusive: false,
                                                 autoDelete: false);

            advancedBus.Consume(queue, x => x
                .Add<TMessage>((message, info) => handler(message.Body)));
        }
    }
}
