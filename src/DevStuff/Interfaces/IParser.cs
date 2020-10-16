using System.Buffers;

namespace DevStuff.Interfaces
{
    public interface IParser
    {
        bool TryParse(ref ReadOnlySequence<byte> buffer,
         out Message message, out int bytesConsumed, string transportName);
    }
}