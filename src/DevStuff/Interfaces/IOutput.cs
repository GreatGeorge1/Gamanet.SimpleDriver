using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevStuff.Interfaces
{
    public interface IOutput : IDisposable
    {
        /// <summary>
        /// used in message bus as address
        /// </summary>
        /// <returns></returns>
        string GetName();
        Task WriteAsync(byte[] message, CancellationToken cancellationToken = default);
    }
}