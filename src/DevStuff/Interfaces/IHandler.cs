using System.Threading.Tasks;

namespace DevStuff.Interfaces
{
    public interface IHandler<TMessage>
    {
        Task HandleAsync(TMessage message, string transport);
    }
}