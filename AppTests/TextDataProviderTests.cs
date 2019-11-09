using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using TextProvider;

namespace AppTests
{
    [TestClass]
    internal class TextDataProviderTests
    {
        [TestMethod]
        [DataRow("abc")]
        [DataRow(null)]
        [DataRow("")]
        public void CreateReader_when_file_is_not_exists(string filePath)
        {
            var provider = new TextDataProvider();

            Assert.ThrowsException<ArgumentException>(() => provider.CreateReader(filePath, CancellationToken.None));
        }
    }
}
