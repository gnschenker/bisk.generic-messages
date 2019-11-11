using System;

namespace bisk.MessageBus
{
    public interface IConsumer : IDisposable
    {
        void Subscribe<TMessage>(Action<TMessage> handler) where TMessage: class;
    }
}