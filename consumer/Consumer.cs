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
        const string EXCHANGE_NAME = "bisk.exchange";
        const string QUEUE_NAME = "durable.sample.queue";
        const string QUEUE_NAME_GENERIC = "durable.sample.queue.generic";
        private static readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private static readonly Random rnd = new Random((int)DateTime.Now.Ticks);

        static void Main(string[] args)
        {
            var key = args.Length > 0 ? args[0].ToLowerInvariant() : "rabbitmq";

            //Task.Factory.StartNew(() => Subscribe(key));
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
                case "rabbitmq": return new RabbitMqConsumer(QUEUE_NAME_GENERIC, new JsonSerDes());
                case "easynetq": return new EasyNetQConsumer(QUEUE_NAME_GENERIC);
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

        private static void Subscribe(string key)
        {
            using (var consumer = GetConsumer(key))
            {
                consumer.Subscribe<TextMessage>(msg =>
                {
                    Console.WriteLine($" [x] Received {msg}");
                    // Simulate processing
                    Thread.Sleep(rnd.Next(100, 2000));
                });
                _waitHandle.WaitOne();
            }
        }
    }
}
