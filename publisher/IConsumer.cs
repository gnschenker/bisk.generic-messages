using System;
using System.Threading;

namespace publisher
{
    public interface IConsumer : IDisposable
    {
        void Subscribe<TMessage>(WaitHandle waitHandle, Action<TMessage> handler) where TMessage: class;
    }
}