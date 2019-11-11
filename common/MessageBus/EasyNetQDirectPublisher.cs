using EasyNetQ;
using EasyNetQ.Topology;
using System;

namespace bisk.MessageBus
{
    public class EasyNetQDirectPublisher : IPublisher
    {
        private IAdvancedBus advancedBus;
        private readonly IExchange exchange;
        private readonly string RABBITMQ_HOST =
            Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        private readonly string EXCHANGE_NAME =
            Environment.GetEnvironmentVariable("EXCHANGE_NAME") ?? "bisk.sample.exchange";

        public EasyNetQDirectPublisher()
        {
            Console.WriteLine("*** Using EasyNetQ Direct Publisher ***");
            Console.WriteLine($"***** RabbitMQ Host: {RABBITMQ_HOST}");
            Console.WriteLine($"***** Exchange Name: {EXCHANGE_NAME}");

            advancedBus = RabbitHutch.CreateBus($"host={RABBITMQ_HOST}").Advanced;
            // create a direct exchange
            exchange = advancedBus.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct);
        }

        public void Dispose()
        {
        }

        public void Publish<TMessage>(TMessage message) where TMessage : class
        {
            // Serialization to JSON happens implicitely here!
            var msg = new Message<TMessage>(message);
            msg.Properties.AppId = "Sample EasyNetQ Publisher";
            var routingKey = typeof(TMessage).FullName;
            advancedBus.Publish(exchange, routingKey, false, msg);
        }
    }
}
