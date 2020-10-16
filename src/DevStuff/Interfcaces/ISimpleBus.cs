using System;
using System.Threading;
using System.Threading.Tasks;
using DevStuff.Constraints;

namespace DevStuff.Interfcaces
{
    public interface ISimpleBus<TMessage> : IDisposable
    {
        void AddInput(IInput<TMessage> input);
        void RemoveInput(IInput<TMessage> input);
        void AddOutput(IOutput output);
        void RemoveOutput(IOutput output);

        Task PushAsync(TMessage message, string output, CancellationToken cancellationToken = default);
        void Subscribe<THandler>(Commands command)
            where THandler : IHandler<TMessage>;
        void Unsubscribe<THandler>(Commands command)
            where THandler : IHandler<TMessage>;
    }
}