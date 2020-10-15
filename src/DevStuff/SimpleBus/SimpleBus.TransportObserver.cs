using System;

namespace DevStuff
{
    public partial class SimpleBus
    {
        private class InputObserver<TMessage> : IObserver<TMessage>
        {
            public string TransportName { get; protected set; }
            public event EventHandler<TransportMessageEventArgs<TMessage>> NextReceived;
            public event EventHandler<Exception> ExceptionReceived;
            public event EventHandler Completed;
            private IDisposable cancellation;

            public InputObserver(string transportName)
            {
                TransportName = transportName ?? throw new ArgumentNullException(nameof(transportName));
            }

            public virtual void Subscribe(IInput<TMessage> transport)
            {
                cancellation = transport.Subscribe(this);
            }
            public virtual void Unsubscribe()
            {
                cancellation.Dispose();
            }

            public void OnCompleted()
            {
                EventHandler handler = Completed;
                handler?.Invoke(this, EventArgs.Empty);
            }

            public void OnError(Exception error)
            {
                EventHandler<Exception> handler = ExceptionReceived;
                handler?.Invoke(this, error);
            }

            public void OnNext(TMessage value)
            {
                EventHandler<TransportMessageEventArgs<TMessage>> handler = NextReceived;
                handler?.Invoke(this, new TransportMessageEventArgs<TMessage>(value, TransportName));
            }
        }
    }
}