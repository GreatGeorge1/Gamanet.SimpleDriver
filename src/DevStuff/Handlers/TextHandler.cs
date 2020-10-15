using System.Threading.Tasks;

namespace DevStuff
{
    public class TextHandler : IHandler<Message>
    {
        private readonly ISimpleBus<Message> _bus;
        public TextHandler(ISimpleBus<Message> simpleBus)
        {
            _bus = simpleBus ?? throw new System.ArgumentNullException(nameof(simpleBus));
        }
        public Task HandleAsync(Message message, string transport)
        {
            throw new System.NotImplementedException();
        }
    }
}