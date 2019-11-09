using Contracts;
using System;
using System.IO;
using System.Threading;

namespace TextProvider
{
    public class TextDataProvider : ITextProvider
    {
        private const int FILE_READ_BUFFER_SIZE = 16 * 1024;
        public string SearchPatern => "*.txt";

        public IReader CreateReader(string filePath, CancellationToken token)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"File {filePath} isn't exists");
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, FILE_READ_BUFFER_SIZE, true);
            return new TextReader(stream, FILE_READ_BUFFER_SIZE, token);
        }
    }
}
