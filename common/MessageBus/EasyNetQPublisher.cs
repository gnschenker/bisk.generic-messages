using bisk.serdes;
using EasyNetQ;
using EasyNetQ.Topology;
using System;

namespace bisk.MessageBus
{
    public class EasyNetQPublisher : IPublisher
    {
        private IAdvancedBus advancedBus;
        private readonly string RABBITMQ_HOST =
            Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        private readonly string QUEUE_NAME =
            Environment.GetEnvironmentVariable("QUEUE_NAME") ?? "bisk.sample.queue";

        public EasyNetQPublisher()
        {
            Console.WriteLine("*** Using EasyNetQ Publisher");
            Console.WriteLine($"***** RabbitMQ Host: {RABBITMQ_HOST}");
            Console.WriteLine($"***** Queue Name:    {QUEUE_NAME}");

            advancedBus = RabbitHutch.CreateBus($"host={RABBITMQ_HOST}").Advanced;
            // declare a durable queue
            var queue = advancedBus.QueueDeclare(QUEUE_NAME, 
                                                 durable: true,
                                                 exclusive: false,
                                                 autoDelete: false);
        }

        public void Dispose()
        {
        }

        public void Publish<TMessage>(TMessage message) where TMessage : class
        {
            // Serialization to JSON happens implicitely here!
            var msg = new Message<TMessage>(message);
            msg.Properties.AppId = "Sample EasyNetQ Publisher";
            advancedBus.Publish(Exchange.GetDefault(), QUEUE_NAME, false, msg);
        }
    }
}
