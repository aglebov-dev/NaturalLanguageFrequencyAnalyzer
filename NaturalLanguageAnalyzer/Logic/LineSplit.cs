using Contracts;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace NaturalLanguageAnalyzer.Logic
{
    public static class LineSplit
    {
        public static IEnumerable<(int lenght, byte[] word)> Split(byte[] source)
        {
            if (source != null)
            {
                var offset = 0;
                for (int i = 0; i < source.Length; i++)
                {
                    var value = source[i];
                    if (value == 0 || value == Constants.SPACE || value == Constants.R || value == Constants.N)
                    {
                        if (i > offset)
                        {
                            yield return CreateData(source, offset, i);
                        }
                        offset = i + 1;
                    }
                }

                var data = CreateData(source, offset, source.Length);
                if (data.lenght > 0)
                {
                    yield return CreateData(source, offset, source.Length);
                }
            }
        }

        private static (int lenght, byte[] word) CreateData(byte[] source, int offset, int i)
        {
            var length = i - offset;
            var data = ArrayPool<byte>.Shared.Rent(length);
            Array.Copy(source, offset, data, 0, length);

            return (length, data);
        }

        internal static void Clear(IEnumerable<(int lenght, byte[] word)> words)
        {
            foreach (var value in words)
            {
                ArrayPool<byte>.Shared.Return(value.word, false);
            }
        }
    }
}
