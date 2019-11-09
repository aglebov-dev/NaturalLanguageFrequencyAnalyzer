using System;
using System.IO;
using System.Threading;
using Autofac;
using CommandLine;
using System.Diagnostics;
using NaturalLanguageAnalyzer.CommandLine;
using NaturalLanguageAnalyzer.Logic;

namespace NaturalLanguageAnalyzer
{
    class Program
    {
        private static IContainer _container;
        private static CancellationTokenSource _cts;

        static int Main(string[] args)
        {
            _container = CompositionRoot.CreateContainer();
            _cts = new CancellationTokenSource();
            Console.CancelKeyPress += Canceled;

            return Parser.Default
                .ParseArguments<Command>(args)
                .MapResult((Command command) => Run(command, _cts.Token), errs => -1);
        }

        private static void Canceled(object sender, ConsoleCancelEventArgs e)
        {
            Console.CancelKeyPress -= Canceled;
            if (e.SpecialKey == ConsoleSpecialKey.ControlC || e.SpecialKey == ConsoleSpecialKey.ControlBreak)
            {
                _cts.Cancel();
            }
        }

        private static int Run(Command command, CancellationToken token)
        {
            var stopwatch = Stopwatch.StartNew();
            var process = _container.Resolve<Processor>();

            if (!Directory.Exists(command.FolderPath))
            {
                Console.WriteLine("Source folder isn't exists");
                return -1;
            }
            if (!Directory.Exists(command.FolderPath))
            {
                Console.WriteLine("Folder of report isn't exists");
                return -1;
            }

            try
            {
                var options = new ReadOptions
                {
                    FolderPath = command.FolderPath,
                    ReportPath = command.ReportPath
                };

                process.StartAsync(options, token).Wait();
                Console.WriteLine($"Total time {stopwatch.ElapsedMilliseconds:n0} ms");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return -1;
            }
        }
    }
}
