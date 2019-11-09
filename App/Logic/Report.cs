using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace App.Logic
{
    internal class Report
    {
        private readonly Encoding _encoding;
        private readonly TreeNode _root;
        private const byte COMMA = 0x2C;
        private const byte NEW_LINE = 0x0A;

        public Report()
        {
            _encoding = Encoding.GetEncoding("Windows-1251");
            _root = TreeNode.CreateRoot();
        }

        public void Add((int lenght, byte[] word) data)
        {
            if (data.word?.Length > 0)
            {
                _root.Add(data.lenght, data.word);
            }
        }

        internal void AddRange(IEnumerable<(int length, byte[] bytes)> words)
        {
            words = words ?? throw new ArgumentNullException(nameof(words));
            words.AsParallel().ForAll(Add);
        }

        public void Flush(Stream stream)
        {
            stream = stream ?? throw new ArgumentNullException(nameof(stream));

            var words = _root.GetWords().OrderByDescending(x => x).ToArray();
            for (int i = 0; i < words.Length - 1; i++)
            {
                WriteToStream(stream, words, i);
                stream.WriteByte(NEW_LINE);
            }

            if (words.Length > 0)
            {
                WriteToStream(stream, words, words.Length - 1);
            }
        }

        private void WriteToStream(Stream stream, TreeNode[] words, int i)
        {
            var word = words[i];
            var bytes = ArrayPool<byte>.Shared.Rent(word.Length);
            word.Populate(bytes);

            var count = _encoding.GetBytes(word.Count.ToString());

            stream.Write(bytes, 0, word.Length);
            stream.WriteByte(COMMA);
            stream.Write(count, 0, count.Length);

            ArrayPool<byte>.Shared.Return(bytes);
        }
    }
}
