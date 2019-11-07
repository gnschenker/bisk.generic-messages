using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace publisher
{
    public class RabbitMqPublisher : IPublisher
    {
        private string _queueName;
        private readonly ISerDes serdes;

        public RabbitMqPublisher(string queueName, ISerDes serdes)
        {
            _queueName = queueName;
            this.serdes = serdes;
        }

        public void Dispose()
        {
        }

        public void Publish<TMessage>(TMessage message) where TMessage : class
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

                    string messageJson = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(messageJson);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "sample.acme.com",
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine($" [x] Sent {messageJson}");
                }
            }
        }
    }
}
