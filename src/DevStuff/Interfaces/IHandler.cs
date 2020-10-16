using System.Threading.Tasks;

namespace DevStuff.Interfcaces
{
    public interface IHandler<TMessage>
    {
        Task HandleAsync(TMessage message, string transport);
    }
}