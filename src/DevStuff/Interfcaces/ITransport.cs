using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevStuff
{
    public interface ITransport<TMessage> : IInput<TMessage>, IOutput
    {
        void Reset();
    }
    public interface IInput<TMessage> : IDisposable, IObservable<TMessage>
    {
        string GetName();
        Task StartListenAsync();
        Task StopListenAsync();
    }
    public interface IOutput : IDisposable
    {
        string GetName();
        Task WriteAsync(byte[] message, CancellationToken cancellationToken = default);
    }
}