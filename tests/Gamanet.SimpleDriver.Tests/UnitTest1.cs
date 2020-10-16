using System;
using System.Buffers;
using System.Text;
using DevStuff;
using Xunit;
using System.Linq;

namespace Gamanet.SimpleDriver.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void TryParse_Returns_True_And_Outputs_Message()
        {
            var parser = new Parser();
            var sequence = new ReadOnlySequence<byte>(Encoding.ASCII.GetBytes("PT:text:E"));
            Assert.True(parser.TryParse(ref sequence, out Message message, out int consumed, "default"));
            Assert.True(sequence.Length == consumed);
            Assert.True(message.Command == (byte)'T');
            var test = message.Body;
            Assert.True(Encoding.ASCII.GetString(message.Body.ToArray()).Equals("text"));
            Assert.True(message.TransportName.Equals("default"));
        }

        [Fact]
        public void TryParse_Returns_True_And_Outputs_Null()
        {
            var parser = new Parser();
            var sequence = new ReadOnlySequence<byte>(Encoding.ASCII.GetBytes("PE"));
            Assert.True(parser.TryParse(ref sequence, out Message message, out int consumed, "default"));
            Assert.True(message is null);
            Assert.True(sequence.Length == consumed);
        }
        
        [Fact]
        public void TryParse_Returns_True_And_Outputs_Null2()
        {
            var parser = new Parser();
            var sequence = new ReadOnlySequence<byte>(Encoding.ASCII.GetBytes("EtrashP"));
            Assert.True(parser.TryParse(ref sequence, out Message message, out int consumed, "default"));
            Assert.True(message is null);
            Assert.True(sequence.Length == consumed);
        }
        
        [Fact]
        public void TryParse_Returns_True_And_Outputs_Null3()
        {
            var parser = new Parser();
            var sequence = new ReadOnlySequence<byte>(Encoding.ASCII.GetBytes("P:trash:E"));
            Assert.True(parser.TryParse(ref sequence, out Message message, out int consumed, "default"));
            Assert.True(message is null);
            Assert.True(sequence.Length == consumed);
        }
        
        [Fact]
        public void TryParse_Returns_False_And_Outputs_Null()
        {
            var parser = new Parser();
            var sequence = new ReadOnlySequence<byte>(Encoding.ASCII.GetBytes("trash"));
            Assert.False(parser.TryParse(ref sequence, out Message message, out int consumed, "default"));
            Assert.True(message is null);
            Assert.True(consumed==0);
        }
        [Fact]
        public void TryParse_Returns_False_And_Outputs_Null2()
        {
            var parser = new Parser();
            var sequence = new ReadOnlySequence<byte>(Encoding.ASCII.GetBytes("t"));
            Assert.False(parser.TryParse(ref sequence, out Message message, out int consumed, "default"));
            Assert.True(message is null);
            Assert.True(consumed==0);
        }
    }
}

