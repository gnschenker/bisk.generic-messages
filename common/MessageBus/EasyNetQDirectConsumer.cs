using EasyNetQ;
using EasyNetQ.Topology;
using System;

namespace bisk.MessageBus
{
    public class EasyNetQDirectConsumer : IConsumer
    {
        private readonly string RABBITMQ_HOST = 
            Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost"; 
        private readonly string EXCHANGE_NAME =
            Environment.GetEnvironmentVariable("EXCHANGE_NAME") ?? "bisk.sample.exchange";
        private readonly string ROUTING_KEY =
            Environment.GetEnvironmentVariable("ROUTING_KEY") ?? "bisk.messages.GenericMessage";
        private readonly IAdvancedBus advancedBus;

        public EasyNetQDirectConsumer()
        {
            Console.WriteLine("*** Using EasyNetQ Direct Consumer");
            Console.WriteLine($"***** RabbitMQ Host: {RABBITMQ_HOST}");
            Console.WriteLine($"***** Exchange Name: {EXCHANGE_NAME}");
            Console.WriteLine($"***** Routing Key:   {ROUTING_KEY}");

            advancedBus = RabbitHutch.CreateBus($"host={RABBITMQ_HOST}").Advanced;
        }

        public void Dispose()
        {
            if(advancedBus != null)
                advancedBus.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Subscribe<TMessage>(Action<TMessage> handler) where TMessage : class
        {
            var queue = advancedBus.QueueDeclare();
            var exchange = advancedBus.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct);
            var binding = advancedBus.Bind(exchange, queue, ROUTING_KEY);
            advancedBus.Consume(queue, x => x
                .Add<TMessage>((message, info) => handler(message.Body)));
        }
    }
}
