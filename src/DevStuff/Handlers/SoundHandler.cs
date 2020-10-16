using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using DevStuff.Interfcaces;

namespace DevStuff.Handlers
{
    public class SoundHandler : IHandler<Message>
    {
        private readonly ISimpleBus<Message> _bus;
        public SoundHandler(ISimpleBus<Message> simpleBus)
        {
            _bus = simpleBus ?? throw new System.ArgumentNullException(nameof(simpleBus));
        }
        public async Task HandleAsync(Message message, string transport)
        {
            var NACK = new Message(0, new List<byte> { 78, 65, 67, 75 }, transport, false);
            var ACK = new Message(0, new List<byte> { 65, 67, 75 }, transport, false);
            var coma = 44;
            var count = 0;
            foreach (var b in message.Body)
            {
                if (b == coma)
                {
                    count++;
                }
            }
            if (count == 0 && count > 1)
            {
                await _bus.PushAsync(NACK, transport)
              .ConfigureAwait(false);
                return;
            }
            string str = Encoding.ASCII.GetString(message.Body.ToArray());
            string[] arr = str.Split(",");
            foreach (var st in arr)
            {
                st.Trim();
            }
            int frequency = 0;
            int duration = 0;
            try
            {
                frequency = int.Parse(arr[0]);
                duration = int.Parse(arr[1]);
            }
            catch (Exception e)
            {
                await _bus.PushAsync(NACK, transport)
             .ConfigureAwait(false);
                return;
            }
            await _bus.PushAsync(ACK, transport)
              .ConfigureAwait(false);
            try
            {
                Console.Beep(frequency, duration);
            }
            catch (PlatformNotSupportedException e)
            {
                Console.WriteLine("Beep on your platform not supported ;(");
            }
        }
    }
}