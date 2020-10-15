using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;

namespace DevStuff
{
    class Program
    {
        public static async Task Main()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<SimpleBus>().As<ISimpleBus<Message>>()
                .SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<TextHandler>();
            builder.RegisterType<SoundHandler>();
            var container = builder.Build();
            var bus = container.BeginLifetimeScope().Resolve<ISimpleBus<Message>>();
            bus.Subscribe<TextHandler>(Commands.Text);
            bus.Subscribe<SoundHandler>(Commands.Sound);
            var ct = new Transport();
            bus.AddInput(ct);
            bus.AddOutput(ct);

            await ct.StartListenAsync().ConfigureAwait(false);
            CancellationTokenSource cts = new CancellationTokenSource();
            while (true)
            {
                while (!Console.KeyAvailable)
                {
                    await Task.Delay(250).ConfigureAwait(false);
                }
                var cki = Console.ReadKey(true);
                if (cki.Key.ToString() == nameof(ConsoleKey.Backspace))
                {
                    ClearCurrentConsoleLine();
                    ct.Reset();
                    continue;
                }
                else if (cki.Key.ToString() == nameof(ConsoleKey.Enter))
                {
                    continue;
                }
                else if (cki.Key.ToString() == nameof(ConsoleKey.Escape))
                {
                    cts.Cancel();
                    ct.Dispose();
                    break;
                }
                int input = cki.KeyChar;
                if (input < 32 && input > 127)
                {
                    continue;
                }
                Console.Write((char)input);
                if (input != -1)
                {
                    await ct.WriteAsync(new byte[] { (byte)input }, cts.Token).ConfigureAwait(false);
                }
            }
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}
