using System.Buffers;

namespace DevStuff.Interfaces
{
    public interface IParser<TMessage>
    {
        bool TryParse(ref ReadOnlySequence<byte> buffer,
         out TMessage message, out int bytesConsumed, string transportName);
    }
}