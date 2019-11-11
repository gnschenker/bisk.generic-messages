using bisk.MessageBus;
using bisk.messages;
using bisk.serdes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace publisher
{
    class Publisher
    {
        private static readonly Random rnd = new Random((int)DateTime.Now.Ticks);
        private static readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            var key = args.Length > 0 ? args[0].ToLowerInvariant() : "rabbitmq";
            
            // *** Test with message corresponding to Bisk's requirements
            Task.Factory.StartNew(() => PublishGeneric(key));

            Console.CancelKeyPress += (sender, _) =>
            {
                Console.WriteLine("Exit");
                _waitHandle.Set();
            };
            _waitHandle.WaitOne();
        }

        private static IPublisher GetPublisher(string key)
        {
            switch(key)
            {
                case "rabbitmq": return new RabbitMqPublisher(new JsonSerDes());
                case "easynetq": return new EasyNetQPublisher();
                case "rabbitmqdirect": return new RabbitMqDirectPublisher(new JsonSerDes());
                case "easynetqdirect": return new EasyNetQDirectPublisher();
                default:
                    throw new ArgumentException("Not a know publisher: {key}");
            }
        }

        private static void PublishGeneric(string key)
        {
            using (var publisher = GetPublisher(key))
            {
                for (int i = 1; i <= 1000000; i++)
                {
                    var payload = new TextMessage { Text = $"{i}: Hello World..." };
                    var msg = GetGenericMessage(payload);

                    publisher.Publish(msg);
                    
                    Console.WriteLine(" [x] Sent wrapped message {0}", msg.Message.Body);
                    // Simulate real world
                    Thread.Sleep(rnd.Next(0, 500));
                }

            }
        }

        private static GenericMessage GetGenericMessage<TMessage>(TMessage payload) where TMessage: class
        {
            var msg = new GenericMessage
            {
                Message = new GenericMessage.MessageBody
                {
                    CorrelationId = Guid.NewGuid(),
                    ResponseTimeStamp = DateTime.Now,
                    Topic = "SomeTopic"
                },
                Event = new GenericMessage.MessageEvent
                {
                    Replay = true,
                    Rollback = true,
                    NormalOperation = true
                },
                Status = new GenericMessage.MessageStatus
                {
                    Success = true,
                    Error = "Nothing to see here..."
                }
            };
            msg.SetPayload(payload);
            return msg;
        }
    }
}
