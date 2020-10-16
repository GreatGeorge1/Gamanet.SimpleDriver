using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DevStuff.Interfaces;

namespace DevStuff.Handlers
{
    public class TextHandler : IHandler<Message>
    {
        private readonly ISimpleBus<Message> _bus;
        public TextHandler(ISimpleBus<Message> simpleBus)
        {
            _bus = simpleBus ?? throw new ArgumentNullException(nameof(simpleBus));
        }
        public async Task HandleAsync(Message message, string transport)
        {
            await _bus.PushAsync(new Message(0, new List<byte> { 65, 67, 75 }, transport, false), transport)
                .ConfigureAwait(false);
            Console.WriteLine($"TEXT: {Encoding.ASCII.GetString(message.Body.ToArray())}");
        }
    }
}