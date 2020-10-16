using System;
using System.Threading.Tasks;

namespace DevStuff.Interfaces
{
    public interface IInput<TMessage> : IDisposable, IObservable<TMessage>
    {
        /// <summary>
        /// used in message bus as address
        /// </summary>
        /// <returns></returns>
        string GetName();
        Task StartListenAsync();
        Task StopListenAsync();
    }
}