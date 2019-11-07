using System;

namespace publisher
{
    public interface IPublisher : IDisposable
    {
        void Publish<TMessage>(TMessage message) where TMessage : class;
    }
}