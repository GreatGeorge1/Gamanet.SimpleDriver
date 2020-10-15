using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevStuff
{
    public partial class Transport : ITransport<Message>
    {
        private bool isDisposed;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        private readonly Pipe pipe = new Pipe();
        private CancellationTokenSource cts = new CancellationTokenSource();
        private Task readPipeTask;
        private readonly List<IObserver<Message>> observers = new List<IObserver<Message>>();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;
            cts.Cancel();
            pipe.Reader.CancelPendingRead();
            pipe.Writer.CancelPendingFlush();
            cts.Dispose();

            foreach (var observer in observers.ToArray())
            {
                if (observers.Contains(observer))
                {
                    observer.OnCompleted();
                }
            }
            observers.Clear();
            semaphore.Dispose();
            isDisposed = true;
        }

        public async Task WriteAsync(byte[] message, CancellationToken cancellationToken = default)
        {
            await pipe.Writer.WriteAsync(message, cancellationToken).ConfigureAwait(false);
        }

        public Task StartListenAsync()
        {
            readPipeTask = Task.Run(() => ReadPipeAsync(pipe.Reader, cts.Token));
            return Task.CompletedTask;
        }

        public Task StopListenAsync()
        {
            pipe.Reader.CancelPendingRead();
            cts.Cancel();
            return Task.CompletedTask;
        }

        private async Task ReadPipeAsync(PipeReader reader, CancellationToken cancellationToken = default)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    reader.CancelPendingRead();
                    await reader.CompleteAsync();
                    break;
                }
                bool isAny = reader.TryRead(out ReadResult result);
                if (!isAny)
                {
                    continue;
                }
                ReadOnlySequence<byte> buffer = result.Buffer;
                // In the event that no message is parsed successfully, mark consumed
                // as nothing and examined as the entire buffer.
                SequencePosition consumed = buffer.Start;
                SequencePosition examined = buffer.End;
                try
                {
                    if (TryParseMessage(ref buffer, out Message message))
                    {
                        consumed = buffer.GetPosition(message.Length);
                        examined = consumed;
                        foreach (var observer in observers)
                        {
                            observer.OnNext(message);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    foreach (var observer in observers)
                    {
                        observer.OnError(e);
                    }
                }
                finally
                {
                    reader.AdvanceTo(consumed, examined);
                }
            }
        }

        private bool TryParseMessage(
          ref ReadOnlySequence<byte> buffer,
          out Message message)
        {
            if (buffer.Length >= 9)
            {
                message = new Message(84, buffer.ToArray().ToList(), GetName(), (int)buffer.Length);
                return true;
            }
            message = null;
            return false;
        }
        public IDisposable Subscribe(IObserver<Message> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
            return new Unsubscriber(observers, observer);
        }
        public string GetName()
        {
            return "default";
        }

        public void Reset()
        {
            cts.Cancel();
            StopListenAsync().Wait();
            pipe.Writer.CancelPendingFlush();
            pipe.Writer.Complete();
            pipe.Reset();
            cts = new CancellationTokenSource();
            StartListenAsync().Wait();
        }
    }
}