using Microsoft.VisualStudio.TestTools.UnitTesting;
using NaturalLanguageAnalyzer.Logic;
using System.Linq;
using System.Text;

namespace AppTests
{
    [TestClass]
    public class LineSplitTests
    {
        private static Encoding _encoding = Encoding.GetEncoding("Windows-1251");


        [TestMethod]
        public void Split_when_one_word_then_split_same()
        {
            var line = "word";
            var bytes = _encoding.GetBytes(line);

            var words = LineSplit.Split(bytes).ToArray();

            Assert.AreEqual(1, words.Length);

            var result = _encoding.GetString(words[0].word, 0, words[0].lenght);
            Assert.AreEqual(line, result);
        }

        [TestMethod]
        public void Split_when_many_word_then_split_all()
        {
            var line = "A, B and C were sitting on the pipe B fell A tried, who remained on the pipe";
            var bytes = _encoding.GetBytes(line);

            var words = LineSplit.Split(bytes);

            var query =
                from x in words
                select _encoding.GetString(x.word, 0, x.lenght);

            var result = string.Join(" ", query);

            Assert.AreEqual(line, result);
        }

        [TestMethod]
        public void Split_when_only_spaces_word_then_empty_result()
        {
            var line = new string(' ', 100);
            var bytes = _encoding.GetBytes(line);

            var words = LineSplit.Split(bytes);

            Assert.AreEqual(0, words.Count());
        }

        [TestMethod]
        public void Split_when_only_separate_chars_word_then_empty_result()
        {
            var line = "\r\n\n\n\n\r    \r   \n";
            var bytes = _encoding.GetBytes(line);

            var words = LineSplit.Split(bytes);

            Assert.AreEqual(0, words.Count());
        }

        [TestMethod]
        public void Split_when_source_is_null_then_return_empty()
        {
            var xxx = LineSplit.Split(null);

            Assert.AreEqual(0, xxx.Count());
        }
    }
}
