using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using DevStuff.Interfcaces;

namespace DevStuff
{
    public partial class Transport : ITransport<Message>
    {
        private readonly Parser parser = new Parser();
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
                    if (parser.TryParse(ref buffer, out Message message, out int bConsumed, GetName()))
                    {
                        // A single message was successfully parsed so mark the start as the
                        // parsed buffer as consumed. TryParseMessage trims the buffer to
                        // point to the data after the message was parsed.
                        consumed = buffer.GetPosition(bConsumed);

                        // Examined is marked the same as consumed here, so the next call
                        // to ReadSingleMessageAsync will process the next message if there's
                        // one.
                        examined = consumed;
                        Console.WriteLine();
                        if (!(message is null))
                        {
                            foreach (var observer in observers)
                            {
                                observer.OnNext(message);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

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
            pipe.Reader.CancelPendingRead();
            pipe.Reader.Complete();
            pipe.Writer.CancelPendingFlush();
            pipe.Writer.Complete();
            pipe.Reset();
            StopListenAsync().Wait();
            cts = new CancellationTokenSource();
            StartListenAsync().Wait();
        }
    }
}