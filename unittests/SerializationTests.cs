using System;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using publisher;
using bisk.messages;
using bisk.serdes;

namespace unittests
{
    public class SerializationTests
    {
        private readonly ITestOutputHelper output;

        public SerializationTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void ShouldSerializeGenericMessage()
        {
            var serdes = new JsonSerDes();

            var payload = new TextMessage {
                        Text = "Hello world"
                    };

            var msg = GetGenericMessage();
            msg.SetPayload(payload);

            var jsonString = serdes.Stringify(msg);
            output.WriteLine($"{jsonString}");

            Assert.Equal("{\"Message\":{\"CorrelationId\":", jsonString.Substring(0,28));
        }

        [Fact]
        public void ShouldSerializeUsingBiskSerDes()
        {
            var serdes = new BiskSerDes();
            var msg = GetGenericMessage();
            var payload = new TextMessage
            {
                Text = "Hello world"
            };
            var jsonString = serdes.Stringify(msg, payload);
            output.WriteLine($"{jsonString}");

            Assert.Equal("{\"Message\":{\"CorrelationId\":", jsonString.Substring(0, 28));
        }

        [Fact]
        public void ShouldDeserializeUsingBiskSerDes()
        {
            var serdes = new BiskSerDes();
            var msg = GetGenericMessage();
            var payload = new TextMessage
            {
                Text = "Hello world"
            };
            var body = serdes.Serialize(msg, payload);

            var result = serdes.Deserialize<TextMessage>(body);

            Assert.IsType<TextMessage>(result.Payload);
            Assert.Equal(payload.Text, ((TextMessage)result.Payload).Text);
            Assert.Equal(msg.Message.CorrelationId, result.WrapperMessage.Message.CorrelationId);
            Assert.Equal(msg.Message.Topic, result.WrapperMessage.Message.Topic);
            Assert.Equal(msg.Message.ResponseTimeStamp, result.WrapperMessage.Message.ResponseTimeStamp);
            Assert.Equal(msg.Status.Success, result.WrapperMessage.Status.Success);
            Assert.Equal(msg.Status.Error, result.WrapperMessage.Status.Error);
            Assert.Equal(msg.Event.Replay, result.WrapperMessage.Event.Replay);
            Assert.Equal(msg.Event.Rollback, result.WrapperMessage.Event.Rollback);
            Assert.Equal(msg.Event.NormalOperation, result.WrapperMessage.Event.NormalOperation);
        }

        private GenericMessage GetGenericMessage()
        {
            return new GenericMessage
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
        }
    }
}
