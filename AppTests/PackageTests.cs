using Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AppTests
{

    [TestClass]
    internal class PackageTests
    {
        [TestMethod]
        public void Ctor_when_null_arg_then_init_empty_array()
        {
            var package = new Package(null, null);

            Assert.AreEqual(Array.Empty<byte>(), package.TextLine);
        }

        [TestMethod]
        public void Ctor_when_arg_is_array_then_set_property_TextLine()
        {
            var array = new byte[] { 1, 2, 3, 4, 5 };
            var package = new Package(array, null);

            Assert.AreEqual(array, package.TextLine);
        }

        [TestMethod]
        public void Ctor_when_invoke_dispose_then_invoke_delegate()
        {
            var flag = false;
            var package = new Package(null, () => flag = true);
            
            package.Dispose();

            Assert.IsTrue(flag);
        }
    }
}
