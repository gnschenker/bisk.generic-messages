using System;
using System.Threading;
using System.Threading.Tasks;

namespace publisher
{
    class Program
    {
        private static readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);
        const string QUEUE_NAME = "discipline.selected";

        static void Main(string[] args)
        {
            Task.Factory.StartNew(() => Subscribe(QUEUE_NAME));
            Task.Factory.StartNew(() => Publish(QUEUE_NAME));

            Console.CancelKeyPress += (sender, _) =>
            {
                Console.WriteLine("Exit");
                _waitHandle.Set();
            };
            _waitHandle.WaitOne();
        }
        private static IConsumer GetConsumer(string queueName)
        {
            // return new RabbitMqConsumer(queueName, new JsonSerDes());
            return new HelloWorldConsumer(queueName, new JsonSerDes());
        }

        private static IPublisher GetPublisher(string queueName)
        {
            // return new RabbitMqPublisher<TMessage>(queueName, new JsonSerDes());
            return new HelloWorldPublisher(queueName, new JsonSerDes());
        }

        private static void Subscribe(string queueName)
        {
            using (var consumer = GetConsumer(queueName))
            {
                // _consumer.Subscribe<TextMessage>(_waitHandle, msg =>
                consumer.Subscribe<DisciplineSelected>(_waitHandle, msg =>
                {
                    Console.WriteLine($" [x] Received {msg}");
                });
            }
        }

        private static void Publish(string queueName)
        {
            var applicationId = Guid.NewGuid();
            using (var publisher = GetPublisher(queueName))
            {
                for (int i = 0; i < 5000; i++)
                {
                    var msg = new DisciplineSelected { ApplicationId = applicationId, DisciplineId = Guid.NewGuid() };
                    // var msg = new TextMessage { Text = "Hello World" };
                    publisher.Publish(msg);
                    Thread.Sleep(500);
                }
            }
        }
    }
}
