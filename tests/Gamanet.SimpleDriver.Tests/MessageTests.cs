using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevStuff;
using DevStuff.Constraints;
using Xunit;

namespace Gamanet.SimpleDriver.Tests
{
    public class MessageTests
    {
        [Fact]
        public void Message_Constructed_WithCommand()
        {
            byte command = (byte) 'T';
            List<byte> body = Encoding.ASCII.GetBytes("some_text").ToList();
            var message = new Message(command, body, "default", true);
            Assert.True(message.WithCommand);
            Assert.True(message.Command==command);
            Assert.True(message.Body.Equals(body));
            Assert.True(message.TransportName == "default");
        }
        
        [Fact]
        public void Message_Constructed_WithoutCommand()
        {
            List<byte> body = Encoding.ASCII.GetBytes("some_text").ToList();
            var message = new Message(0, body, "default", false);
            Assert.False(message.WithCommand);
            Assert.True(message.Body.Equals(body));
            Assert.True(message.TransportName == "default");
        }
        
        [Fact]
        public void Message_WithCommand_ToArray()
        {
            byte command = (byte) 'T';
            List<byte> body = Encoding.ASCII.GetBytes("some_text").ToList();
            var message = new Message(command, body, "default", true);
            Assert.True(message.WithCommand);
            
            var list = new List<byte>();
            list.Add((byte)Protocol.StartByte);
            list.Add(command);
            list.Add((byte)Protocol.Separator);
            list.AddRange(body);
            list.Add((byte)Protocol.Separator);
            list.Add((byte)Protocol.EndByte);
            var etalon = list.ToArray();
            var arr = message.ToArray();
            Assert.Equal(arr,etalon);
        }
        
        [Fact]
        public void Message_WithoutCommand_ToArray()
        {
            List<byte> body = Encoding.ASCII.GetBytes("some_text").ToList();
            var message = new Message(0, body, "default", false);
            Assert.False(message.WithCommand);
            
            var list = new List<byte>();
            list.Add((byte)Protocol.StartByte);
            list.Add((byte)Protocol.Separator);
            list.AddRange(body);
            list.Add((byte)Protocol.Separator);
            list.Add((byte)Protocol.EndByte);
            var etalon = list.ToArray();
            var arr = message.ToArray();
            Assert.Equal(arr,etalon);
        }
    }
}