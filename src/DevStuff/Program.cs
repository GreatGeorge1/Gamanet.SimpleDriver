using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DevStuff.Constraints;
using DevStuff.Handlers;
using DevStuff.Interfaces;

namespace DevStuff
{
    class Program
    {
        public static IContainer AutofacContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<SimpleBus>().As<ISimpleBus<Message>>()
                .SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<TextHandler>();
            builder.RegisterType<SoundHandler>();
            return builder.Build();
        }
        public static async Task Main()
        {
            var container = AutofacContainer();
            var bus = container.BeginLifetimeScope()
                .Resolve<ISimpleBus<Message>>();
            bus.Subscribe<TextHandler>(Commands.Text);
            bus.Subscribe<SoundHandler>(Commands.Sound);
            var ct = new Transport(new Parser());
            bus.AddInput(ct);
            bus.AddOutput(new ConsoleOutput());

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
                    bus.Dispose();
                    ct.Dispose();
                    container.Dispose();
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
