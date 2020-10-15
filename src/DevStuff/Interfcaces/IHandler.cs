using System.Threading.Tasks;

namespace DevStuff
{
    public interface IHandler<TMessage>
    {
        Task HandleAsync(TMessage message, string transport);
    }
}