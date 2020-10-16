using System;
using System.Threading;
using System.Threading.Tasks;
using DevStuff.Constraints;

namespace DevStuff.Interfaces
{
    /// <summary>
    /// Simple message bus, intended to work as mediator
    /// between IO and Command handlers.
    /// Handlers should use PushAsync to send response.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface ISimpleBus<TMessage> : IDisposable
    {
        void AddInput(IInput<TMessage> input);
        void RemoveInput(IInput<TMessage> input);
        void AddOutput(IOutput output);
        void RemoveOutput(IOutput output);

        /// <summary>
        /// Sends message to registered output
        /// </summary>
        /// <param name="message"></param>
        /// <param name="output">output name</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PushAsync(TMessage message, string output, CancellationToken cancellationToken = default);
        void Subscribe<THandler>(Commands command)
            where THandler : IHandler<TMessage>;
        void Unsubscribe<THandler>(Commands command)
            where THandler : IHandler<TMessage>;
    }
}