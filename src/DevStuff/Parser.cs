using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevStuff
{
    public class Parser
    {
        public byte StartByte { get; protected set; } = 80;
        public byte EndByte { get; protected set; } = 69;
        public byte Separator { get; protected set; } = 58;

        private List<Message> Parse(byte[] buffer, int startPos, int length)
        {
            var mlist = new List<Message>();

            var arr = new byte[length];
            Array.Copy(buffer, startPos, arr, 0, length);
            var str = System.Text.Encoding.ASCII.GetString(arr);
            Console.WriteLine(str);
            return mlist;
        }
    }
}