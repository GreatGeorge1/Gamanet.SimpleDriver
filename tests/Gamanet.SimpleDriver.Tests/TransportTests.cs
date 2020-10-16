using System;
using DevStuff;
using Xunit;
using DevStuff.Interfaces;

namespace Gamanet.SimpleDriver.Tests
{
    public class TransportTests
    {
        [Fact]
        public void Transport_Parser_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(()=>new Transport(null));
        }
    }
}