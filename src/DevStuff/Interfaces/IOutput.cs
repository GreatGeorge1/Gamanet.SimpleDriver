using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevStuff.Interfcaces
{
    public interface IOutput : IDisposable
    {
        string GetName();
        Task WriteAsync(byte[] message, CancellationToken cancellationToken = default);
    }
}