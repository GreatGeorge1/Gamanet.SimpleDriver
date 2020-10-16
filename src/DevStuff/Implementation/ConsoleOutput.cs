using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevStuff.Interfaces;

namespace DevStuff
{
    /// <summary>
    /// Simple output for use with message bus
    /// </summary>
    public class ConsoleOutput : IOutput
    {
        public void Dispose()
        {
        }

        public string GetName()
        {
            return "default";
        }

        public Task WriteAsync(byte[] message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine(Encoding.ASCII.GetString(message));
            return Task.CompletedTask;
        }
    }
}