using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevStuff.Interfcaces;

namespace DevStuff
{
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