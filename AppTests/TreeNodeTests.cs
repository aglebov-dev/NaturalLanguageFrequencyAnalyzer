using Microsoft.VisualStudio.TestTools.UnitTesting;
using NaturalLanguageAnalyzer.Logic;
using System;
using System.Linq;
using System.Text;

namespace AppTests
{
    [TestClass]
    public class TreeNodeTests
    {
        private static Encoding _encoding = Encoding.GetEncoding("Windows-1251");

        [TestMethod]
        public void Add_when_length_more_that_array_then_add()
        {
            var text = "hello".ToUpper();
            var bytes = _encoding.GetBytes(text);
            var root = TreeNode.CreateRoot();


            root.Add(12, bytes);

            var words = root.GetWords().ToArray();
            Assert.AreEqual(1, words.Length);

            var resultBytes = new byte[bytes.Length];
            words[0].Populate(resultBytes);
            CollectionAssert.AreEquivalent(bytes, resultBytes);
        }

        [TestMethod]
        public void Add_when_length_less_that_array_then_throw()
        {
            var bytes = _encoding.GetBytes("hello");
            var root = TreeNode.CreateRoot();

            root.Add(12, bytes);
            var words = root.GetWords().ToArray();

            Assert.ThrowsException<ArgumentException>(() => words[0].Populate(new byte[1]));
        }


        [TestMethod]
        public void Add_when_data_is_multiline_then_equals_test_data()
        {
            var line = "A, B and C were sitting \non the\r pipe B fell A   \n    tried, who remained on the pipe";
            var bytes = _encoding.GetBytes(line);
            var words = LineSplit.Split(bytes);

            var root = TreeNode.CreateRoot();
            foreach (var word in words)
            {
                root.Add(word.lenght, word.word);
            }

            var treeWords = root.GetWords()
                .Select(x =>
                {
                    var array = new byte[x.Length];
                    x.Populate(array);
                    return _encoding.GetString(array);
                })
                .ToArray();

            var lineResult = line
                .Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(x => x)
                .Select(x => x.Key.ToUpper())
                .ToArray();

            CollectionAssert.AreEquivalent(lineResult, treeWords);
        }

        [TestMethod]
        public void Add_when_data_is_multiline_then_count_is_correct()
        {
            var line = "A, B and C were sitting \non the\r pipe B fell A   \n    tried, who remained on the pipe";
            var bytes = _encoding.GetBytes(line);
            var words = LineSplit.Split(bytes);
            var expectedCount = line
                .Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(x => x)
                .Count();

            var root = TreeNode.CreateRoot();
            foreach (var word in words)
            {
                root.Add(word.lenght, word.word);
            }

            var count = root.GetWords().Count();

            Assert.AreEqual(expectedCount, count);
        }

        [TestMethod]
        public void Add_when_data_is_multiline_then_correct_length()
        {
            var line = "A, B and C were sitting \non the\r pipe B fell A   \n    tried, who remained on the pipe";
            var bytes = _encoding.GetBytes(line);
            var words = LineSplit.Split(bytes);

            var root = TreeNode.CreateRoot();
            foreach (var word in words)
            {
                root.Add(word.lenght, word.word);
            }

            var treeWords = root.GetWords()
                .Select(x =>
                {
                    var array = new byte[x.Length];
                    x.Populate(array);
                    return (x, _encoding.GetString(array));
                })
                .ToArray();

            foreach (var item in treeWords)
            {
                Assert.AreEqual(item.x.Length, item.Item2.Length);
            }
        }
    }
}
