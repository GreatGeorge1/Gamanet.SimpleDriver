using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Autofac;
using DevStuff.Constraints;
using DevStuff.Interfaces;

namespace DevStuff
{
    public partial class SimpleBus : ISimpleBus<Message>
    {
        private readonly string AUTOFAC_SCOPE_NAME = "simple_bus";
        private readonly ILifetimeScope _autofac;
        private readonly List<InputObserver<Message>> observers = new List<InputObserver<Message>>();
        private readonly List<IOutput> outputs = new List<IOutput>();
        private readonly Dictionary<Commands, HashSet<SubscriptionInfo>> _handlers = new Dictionary<Commands, HashSet<SubscriptionInfo>>();
        private readonly Dictionary<Commands, byte> _commandTypes = Data.GetCommands();
        private bool isDisposed;

        public SimpleBus(ILifetimeScope autofac)
        {
            _autofac = autofac ?? throw new ArgumentNullException(nameof(autofac));
        }

        public void AddInput(IInput<Message> transport)
        {
            var res = observers.Find(x => x.TransportName.Equals(transport.GetName()));
            if (!(res is null))
            {
                return;
            }
            var tobserver = new InputObserver<Message>(transport.GetName());
            tobserver.Subscribe(transport);
            tobserver.NextReceived += OnNext;
            tobserver.Completed += OnCompleted;
            tobserver.ExceptionReceived += OnException;
            observers.Add(tobserver);
        }

        public void RemoveInput(IInput<Message> transport)
        {
            var res = observers.Find(x => x.TransportName.Equals(transport.GetName()));
            if (!(res is null))
            {
                res.Unsubscribe();
                observers.Remove(res);
            }
        }

        public void AddOutput(IOutput output)
        {
            var res = outputs.Find(x => x.GetName().Equals(output.GetName()));
            if (!(res is null))
            {
                return;
            }
            outputs.Add(output);
        }

        public void RemoveOutput(IOutput output)
        {
            var res = outputs.Find(x => x.GetName().Equals(output.GetName()));
            if (!(res is null))
            {
                outputs.Remove(output);
            }
        }

        private async void OnNext(object sender, TransportMessageEventArgs<Message> eventArgs)
        {
            var key = _commandTypes.FirstOrDefault(x => x.Value == eventArgs.Message.Command).Key;
            if (key != Commands.None && _handlers.TryGetValue(key, out HashSet<SubscriptionInfo> hashset))
            {
                using var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME);
                foreach (var sub in hashset)
                {
                    try
                    {
                        var handler = scope.Resolve(sub.HandlerType);
                        if (handler is null) continue;
                        var concreteType = typeof(IHandler<>).MakeGenericType(new Type[] { typeof(Message) });
                        //Debug.WriteLine($"test: {Encoding.ASCII.GetString(eventArgs.Message.Body.ToArray())}");
                        await (Task)concreteType.GetMethod("HandleAsync").Invoke(handler, new object[] { eventArgs.Message, eventArgs.TransportName });
                    }
                    catch (Exception e)
                    {
                        //TODO handling
                    }
                }
            }
        }
        private void OnException(object sender, Exception exception)
        {
            //TODO smh
        }
        private void OnCompleted(object sender, EventArgs e)
        {
            InputObserver<Message> transport = (InputObserver<Message>)sender;
            transport.Unsubscribe();
            observers.Remove(transport);
        }
        public async Task PushAsync(Message message, string output, CancellationToken cancellationToken = default)
        {
            var _output = outputs.Find(x => x.GetName().Equals(output));
            if (_output is null)
            {
                return;
            }
            await _output.WriteAsync(message.ToArray(), cancellationToken).ConfigureAwait(false);
        }

        public void Subscribe<THandler>(Commands command) where THandler : IHandler<Message>
        {
            var res = _commandTypes.TryGetValue(command, out byte charByte);
            if (res)
            {
                var res2 = _handlers.TryGetValue(command, out HashSet<SubscriptionInfo> hashset);
                if (res2)
                {
                    var sub = SubscriptionInfo.Typed(typeof(THandler));
                    if (!hashset.Contains(sub))
                    {
                        hashset.Add(sub);
                    }
                }
                else
                {
                    var hash = new HashSet<SubscriptionInfo>
                    {
                        SubscriptionInfo.Typed(typeof(THandler))
                    };
                    _handlers.Add(command, hash);
                }
            }
        }

        public void Unsubscribe<THandler>(Commands command) where THandler : IHandler<Message>
        {
            var res = _commandTypes.TryGetValue(command, out byte charByte);
            if (res)
            {
                var res2 = _handlers.TryGetValue(command, out HashSet<SubscriptionInfo> hashset);
                if (res2)
                {
                    var sub = SubscriptionInfo.Typed(typeof(THandler));
                    if (hashset.Contains(sub))
                    {
                        hashset.Remove(sub);
                    }
                }
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;
            foreach (var observer in observers)
            {
                observer.Unsubscribe();
            }
            outputs.Clear();
            _handlers.Clear();
            _commandTypes.Clear();
            observers.Clear();
            isDisposed = true;
        }
    }
}