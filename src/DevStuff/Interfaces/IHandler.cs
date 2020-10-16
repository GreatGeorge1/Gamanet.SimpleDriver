using System.Threading.Tasks;

namespace DevStuff.Interfaces
{
    /// <summary>
    /// Command Handler
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IHandler<TMessage>
    {
        Task HandleAsync(TMessage message, string transport);
    }
}