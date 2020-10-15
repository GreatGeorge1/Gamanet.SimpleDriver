using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DevStuff
{
    public class Message
    {
        public Message(byte command, List<byte> body, string transportName, int length)
        {
            Body = body.AsReadOnly() ?? throw new System.ArgumentNullException(nameof(body));
            Command = command;
            TransportName = transportName;
            Length = length;
        }
        public ReadOnlyCollection<byte> Body { get; protected set; }
        public byte Command { get; protected set; }
        public string TransportName { get; protected set; }
        public int Length { get; protected set; }
        public byte[] ToArray()
        {
            throw new System.NotImplementedException();
        }
    }
}