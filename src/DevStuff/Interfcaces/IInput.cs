using System;
using System.Threading.Tasks;

namespace DevStuff.Interfcaces
{
    public interface IInput<TMessage> : IDisposable, IObservable<TMessage>
    {
        string GetName();
        Task StartListenAsync();
        Task StopListenAsync();
    }
}