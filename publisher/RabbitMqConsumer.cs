using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading;

namespace publisher
{
    public class RabbitMqConsumer : IConsumer, IDisposable
    {
        private string _queueName;
        private readonly ISerDes serdes;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMqConsumer(string queueName, ISerDes serdes)
        {
            _queueName = queueName;
            this.serdes = serdes;
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Subscribe<TMessage>(WaitHandle waitHandle, Action<TMessage> handler) where TMessage: class
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) => {
                var msg = serdes.Deserialize<TMessage>(ea.Body);
                handler(msg);
            };
            _channel.BasicConsume(queue: _queueName,
                                    autoAck: true,
                                    consumer: consumer);
            waitHandle.WaitOne();
        }
    }
}
