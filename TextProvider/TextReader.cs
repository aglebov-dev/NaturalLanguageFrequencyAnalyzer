using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Contracts;
using System.Buffers;

namespace TextProvider
{
    internal class TextReader: IReader
    {
        private readonly Stream _stream;
        private readonly int _bufferSize;
        private readonly CancellationToken _token;

        public TextReader(Stream stream, int bufferSize, CancellationToken cancellationToken)
        {
            _stream = stream;
            _bufferSize = bufferSize;
            _token = cancellationToken;
        }

        public IEnumerable<Package> Read()
        {
            int length;
            var buffer = new byte[_bufferSize];
            while (!_token.IsCancellationRequested)
            {
                if ((length = _stream.Read(buffer, 0, buffer.Length)) == 0)
                {
                    break;
                }
                else
                {
                    var index = length - 1;
                    if (length == _bufferSize)
                    {
                        var valuex = buffer[index];
                        
                        while (index > 0 && valuex != Constants.SPACE && valuex != Constants.R && valuex != Constants.N)
                        {
                            valuex = buffer[--index];
                        }

                        if (index == 0)
                        {
                            throw new ArgumentOutOfRangeException("The word is too long");
                        }

                        var offset = length - index;
                        _stream.Seek(-offset + 1, SeekOrigin.Current);
                        length = index + 1;
                    }

                    var value = ArrayPool<byte>.Shared.Rent(length);
                    Array.Clear(value, 0, value.Length);
                    Array.Copy(buffer, 0, value, 0, length);

                    yield return new Package(value, () => ArrayPool<byte>.Shared.Return(value, false));
                }
            }
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
