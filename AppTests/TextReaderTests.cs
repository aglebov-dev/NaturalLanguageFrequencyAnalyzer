using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AppTests
{
    [TestClass]
    internal class TextReaderTests
    {
        private static Encoding _encoding = Encoding.GetEncoding("Windows-1251");

        [TestMethod]
        public void Read_when_text_does_not_fit_in_the_buffer()
        {
            var text = "hello world   ";
            var bytes = _encoding.GetBytes(text);
            var stream = new MemoryStream(bytes);
            var provider = new TextProvider.TextReader(stream, 8, CancellationToken.None);

            var package = provider.Read().ToArray();

            var pack = package[0];
            Assert.AreEqual("hello", _encoding.GetString(pack.TextLine).TrimEnd('\0', ' '));

            pack = package[1];
            Assert.AreEqual("world", _encoding.GetString(pack.TextLine).TrimEnd('\0', ' '));
        }

        [TestMethod]
        public void Read_when_word_is_whitespace_then_read_empty()
        {
            var text = new string(' ', 33);
            var bytes = _encoding.GetBytes(text);
            var stream = new MemoryStream(bytes);
            var provider = new TextProvider.TextReader(stream, 8, CancellationToken.None);

            foreach (var package in provider.Read())
            {
                Assert.AreEqual(string.Empty, _encoding.GetString(package.TextLine).TrimEnd('\0', ' '));
            }
        }

        [TestMethod]
        public void Read_when_word_is_too_long_then_throw_exception()
        {
            var text = new string('x', 8);
            var bytes = _encoding.GetBytes(text);
            var stream = new MemoryStream(bytes);
            var provider = new TextProvider.TextReader(stream, 8, CancellationToken.None);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => provider.Read().ToArray());
        }

    }
}
