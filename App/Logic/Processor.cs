using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Contracts;
using System.IO;

namespace App.Logic
{
    internal class Processor
    {
        private readonly FilePathService _filePathProvider;
        private readonly IEnumerable<ITextProvider> _textProviders;
        private readonly SemaphoreSlim _semaphore;

        public Processor(FilePathService filePathProvider, IEnumerable<ITextProvider> textProviders)
        {
            _filePathProvider = filePathProvider;
            _textProviders = textProviders;
            _semaphore = new SemaphoreSlim(Constants.THREADS_COUNT);
        }

        public async Task StartAsync(ReadOptions options, CancellationToken token)
        {
            var report = new Report();
            var query =
                from provider in _textProviders
                from filePath in _filePathProvider.GetFilePaths(options.FolderPath, provider.SearchPatern, token)
                let reader = provider.CreateReader(filePath, token)
                select CreateTask(reader, report, token);

            await Task.WhenAll(query);

            if (!token.IsCancellationRequested)
            {
                WriteReport(options.ReportPath, report);
            }
        }

        private void WriteReport(string path, Report report)
        {
            var file = new FileInfo(path);
            Directory.CreateDirectory(file.Directory.FullName);
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 16 * 1024, true))
            {
                report.Flush(stream);
            }
        }

        private async Task CreateTask(IReader reader, Report report, CancellationToken token)
        {
            using (await LockAsync(token))
            {
                if (!token.IsCancellationRequested)
                {
                    ReadInternal(reader, report);
                }
            }
        }

        private async Task<IDisposable> LockAsync(CancellationToken token)
        {
            await _semaphore.WaitAsync(token);
            return new SemaphoreWrapper(_semaphore);
        }

        private void ReadInternal(IReader reader, Report report)
        {
            using (reader)
            {
                foreach (var item in reader.Read())
                {
                    using (item)
                    {
                        var words = LineSplit.Split(item.TextLine);
                        report.AddRange(words);
                        LineSplit.Clear(words);
                    }
                }
            }
        }

        private struct SemaphoreWrapper : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;
            public void Dispose() => _semaphore.Release();
            public SemaphoreWrapper(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }
        }
    }
}
