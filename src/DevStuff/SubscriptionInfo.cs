using System;

namespace DevStuff
{
    public class SubscriptionInfo : IEquatable<SubscriptionInfo>
    {
        public Type HandlerType { get; }
        private SubscriptionInfo(Type handlerType)
        {
            HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType));
        }
        public static SubscriptionInfo Typed(Type handlerType)
        {
            return new SubscriptionInfo(handlerType);
        }

        public bool Equals(SubscriptionInfo other)
        {
            return HandlerType.Equals(other.HandlerType);
        }
    }
}