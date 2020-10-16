using System.Collections.Generic;
using DevStuff.Constraints;

namespace DevStuff
{
    public class Message
    {
        public Message(byte command, List<byte> body, string transportName, bool withCommand = true)
        {
            Body = body ?? throw new System.ArgumentNullException(nameof(body));
            Command = command;
            TransportName = transportName;
            WithCommand = withCommand;
        }
        public List<byte> Body { get; protected set; }
        public byte Command { get; protected set; }
        public string TransportName { get; protected set; }
        /// <summary>
        /// Indicates command will be serialized on call ToArray
        /// </summary>
        public bool WithCommand { get; }

        public byte[] ToArray()
        {
            var list = new List<byte>();
            if (WithCommand)
            {
                list.Add((byte)Protocol.StartByte);
                list.Add(Command);
                list.Add((byte)Protocol.Separator);
                list.AddRange(Body);
                list.Add((byte)Protocol.Separator);
                list.Add((byte)Protocol.EndByte);
            }
            else
            {
                list.Add((byte)Protocol.StartByte);
                list.Add((byte)Protocol.Separator);
                list.AddRange(Body);
                list.Add((byte)Protocol.Separator);
                list.Add((byte)Protocol.EndByte);
            }
            return list.ToArray();
        }
    }
}