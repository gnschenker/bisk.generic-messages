using bisk.serdes;
using EasyNetQ;
using EasyNetQ.Topology;
using System;

namespace bisk.MessageBus
{
    public class EasyNetQPublisher : IPublisher
    {
        private string queueName;
        private IAdvancedBus advancedBus;
        private readonly string RABBITMQ_HOST = 
            Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";

        public EasyNetQPublisher(string queueName)
        {
            Console.WriteLine("*** Using EasyNetQ Publisher");
            this.queueName = queueName;
            advancedBus = RabbitHutch.CreateBus($"host={RABBITMQ_HOST}").Advanced;
            // declare a durable queue
            var queue = advancedBus.QueueDeclare(queueName, 
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
            advancedBus.Publish(Exchange.GetDefault(), queueName, false, msg);
        }
    }
}
