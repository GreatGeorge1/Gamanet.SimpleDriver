using System.Buffers;

namespace DevStuff.Interfcaces
{
    public interface IParser
    {
        bool TryParse(ref ReadOnlySequence<byte> buffer,
         out Message message, out int bytesConsumed, string transportName);
    }
}