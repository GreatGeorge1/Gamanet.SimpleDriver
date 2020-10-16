using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DevStuff.Constraints;
using DevStuff.Interfaces;

namespace DevStuff
{
    public class Parser : IParser<Message>
    {
        /// <summary>
        /// If parser recognize broken message eg: start byte goes after end byte,
        /// method return TRUE with NULL message and number of bytes to advance.
        /// If there is not enough data, method returns FALSE with NULL message and 0 bytes to advance.
        /// If packet successfully parsed? method returns TRUE, outputs new Message and Number of bytes
        /// to advance in buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="message">Message, can be null</param>
        /// <param name="bConsumed">consumed bytes from buffer</param>
        /// <param name="transportName">input name, needed fo message meta</param>
        /// <returns></returns>
        public bool TryParse(ref ReadOnlySequence<byte> buffer,
          out Message message, out int bConsumed, string transportName)
        {
            if (buffer.Length < 2)
            {
                message = null;
                bConsumed = 0;
                return false;
            }
            SequenceReader<byte> reader = new SequenceReader<byte>(buffer);
            var buff = buffer.ToArray();
            int start = 0;
            bool startB = false;
            bool endB = false;
            int end = 0;
            for (int i = 0; i < buff.Length; i++)
            {
                if (buff[i] == (byte)Protocol.StartByte && !startB)
                {
                    Debug.WriteLine("start");
                    start = i;
                    startB = true;
                }
                if (buff[i] == (byte)Protocol.EndByte && !endB)
                {
                    Debug.WriteLine($"end {i}");

                    end = i;
                    endB = true;
                }
            }
            if (startB && endB && start > end)
            {
                message = null;
                bConsumed = start + 1;
                return true;
            }
            if (startB && endB && end - start < 3)
            {
                message = null;
                bConsumed = end + 1;
                return true;
            }
            if ((!startB && !endB) || (!endB && !startB))
            {
                message = null;
                bConsumed = 0;
                return false;
            }
            reader.TryAdvanceTo((byte)Protocol.StartByte, true);

            if (reader.TryReadTo(out ReadOnlySequence<byte> result, (byte)Protocol.EndByte, false))
            {
                var arr = result.ToArray();
                var list = new List<byte[]>();
                var seg = 0;
                var prevSeparator = 0;
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] == (byte)Protocol.Separator)
                    {
                        seg++;
                        var newArr = new byte[i - prevSeparator];
                        Array.Copy(arr, prevSeparator, newArr, 0, i - prevSeparator);
                        list.Add(newArr);
                        prevSeparator = i + 1;
                        if (i == 0)
                        {
                            prevSeparator = 0;
                        }
                    }
                }

                if (list.Count >= 2 && list.First().Length > 0)
                {

                    message = new Message(list.First()[0], list.Skip(1).First().ToList(), transportName);
                    bConsumed = (int)reader.Consumed + 1;
                    return true;
                }
                else
                {
                    message = null;
                    bConsumed = (int)reader.Consumed + 1;
                    return true;
                }
            }

            message = null;
            bConsumed = 0;
            return false;
        }
    }
}