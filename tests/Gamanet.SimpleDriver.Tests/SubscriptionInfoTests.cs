using System;
using DevStuff;
using Xunit;
using DevStuff.Interfaces;


namespace Gamanet.SimpleDriver.Tests
{
    public class SubscriptionInfoTests
    {
        [Fact]
        public void SubcsriptionInfo_With_Type_Object()
        {
            var sub = SubscriptionInfo.Typed(typeof(Object));
            Assert.True(sub.HandlerType == typeof(Object));
        }
        
        [Fact]
        public void SubcsriptionInfo_IEquatable_False()
        {
            var sub = SubscriptionInfo.Typed(typeof(IHandler<>));
            var sub2 = SubscriptionInfo.Typed(typeof(IInput<>));
            Assert.False(sub.Equals(sub2));
        }
        
        [Fact]
        public void SubcsriptionInfo_IEquatable_True()
        {
            var sub = SubscriptionInfo.Typed(typeof(IHandler<>));
            var sub2 = SubscriptionInfo.Typed(typeof(IHandler<>));
            Assert.True(sub.Equals(sub2));
        }
    }
}