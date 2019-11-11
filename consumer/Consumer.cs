using bisk.MessageBus;
using bisk.messages;
using bisk.serdes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace consumer
{
    class Consumer
    {
        private static readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private static readonly Random rnd = new Random((int)DateTime.Now.Ticks);

        static void Main(string[] args)
        {
            var key = args.Length > 0 ? args[0].ToLowerInvariant() : "rabbitmq";

            Task.Factory.StartNew(() => SubscribeGeneric(key));

            Console.CancelKeyPress += (sender, _) =>
            {
                Console.WriteLine("Exit");
                _waitHandle.Set();
            };
            _waitHandle.WaitOne();
        }

        private static IConsumer GetConsumer(string key)
        {
            switch (key)
            {
                case "rabbitmq": return new RabbitMqConsumer(new JsonSerDes());
                case "easynetq": return new EasyNetQConsumer();
                case "easynetqdirect": return new EasyNetQDirectConsumer();
                default:
                    throw new ArgumentException("Not a know consumer: {key}");
            }
        }

        private static void SubscribeGeneric(string key)
        {
            using (var consumer = GetConsumer(key))
            {
                consumer.Subscribe<GenericMessage>(msg =>
                {
                    Console.WriteLine($" [x] Received wrapped message {msg.Message.Body}");
                    // Simulate processing
                    Thread.Sleep(rnd.Next(100, 2000));
                });
                _waitHandle.WaitOne();
            }
        }
    }
}
