using Microsoft.VisualStudio.TestTools.UnitTesting;
using NaturalLanguageAnalyzer.Logic;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AppTests
{
    [TestClass]
    public class ReportTests
    {
        private static Encoding _encoding = Encoding.GetEncoding("Windows-1251");

        [TestMethod]
        public void Flush_when_write_any_text_then_report_contains_all_words()
        {
            var line = "A, B and C were sitting on the pipe B fell A tried, who remained on the pipe";
            var lineResult = line.Split(' ').GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => $"{x.Key.ToUpper()},{x.Count()}").ToArray();
            var bytes = _encoding.GetBytes(line);
            var words = LineSplit.Split(bytes);
            var report = new Report();
            var reportStream = new MemoryStream();

            report.AddRange(words);
            report.Flush(reportStream);

            var result = _encoding.GetString(reportStream.ToArray()).Split('\n').ToArray();

            CollectionAssert.AreEquivalent(lineResult, result);
        }

        [TestMethod]
        public void Flush_when_write_multiline_text_then_report_contains_all_words()
        {
            var line = "A, B and C were sitting \non the\r pipe B fell A   \n    tried, who remained on the pipe";
            var lineResult = line
                .Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(x => x).OrderByDescending(x => x.Count())
                .Select(x => $"{x.Key.ToUpper()},{x.Count()}")
                .ToArray();
            var bytes = _encoding.GetBytes(line);
            var words = LineSplit.Split(bytes);
            var report = new Report();
            var reportStream = new MemoryStream();

            report.AddRange(words);
            report.Flush(reportStream);

            var result = _encoding.GetString(reportStream.ToArray()).Split('\n').ToArray();

            CollectionAssert.AreEquivalent(lineResult, result);
        }

        [TestMethod]
        public void Flush_when_write_empty_then_stream_empty()
        {
            var line = "";
            var bytes = _encoding.GetBytes(line);
            var words = LineSplit.Split(bytes);
            var report = new Report();
            var reportStream = new MemoryStream();

            report.AddRange(words);
            report.Flush(reportStream);

            Assert.AreEqual(0, reportStream.Length);
        }

        [TestMethod]
        public void Flush_when_stream_is_null_then_throw()
        {
            var bytes = _encoding.GetBytes("abc");
            var words = LineSplit.Split(bytes);
            var report = new Report();

            report.AddRange(words);

            Assert.ThrowsException<ArgumentNullException>(() => report.Flush(null));
        }

        [TestMethod]
        public void Add_when_try_add_null_bytes_then_nothing()
        {
            var report = new Report();
            report.Add((1, null));

            var reportStream = new MemoryStream();
            report.Flush(reportStream);

            Assert.AreEqual(0, reportStream.Length);
        }

        [TestMethod]
        public void AddRange_when_try_add_null_then_throw()
        {
            var report = new Report();

            Assert.ThrowsException<ArgumentNullException>(() => report.AddRange(null));
        }
    }
}
