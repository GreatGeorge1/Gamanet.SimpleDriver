using System;

namespace DevStuff
{
    public partial class SimpleBus
    {
        private class TransportMessageEventArgs<TMessage> : EventArgs
        {
            public TransportMessageEventArgs(TMessage message, string transportName)
            {
                Message = message;
                TransportName = transportName;
            }

            public TMessage Message { get; protected set; }
            public string TransportName { get; protected set; }
        }
    }
}