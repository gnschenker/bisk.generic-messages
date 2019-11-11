using System;

namespace bisk.MessageBus
{
    public interface IPublisher : IDisposable
    {
        void Publish<TMessage>(TMessage message) where TMessage : class;
    }
}