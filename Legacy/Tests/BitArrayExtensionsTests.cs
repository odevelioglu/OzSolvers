using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzCompress;

namespace Tests
{
    [TestClass]
    public class BitArrayExtensionsTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var list = "11001111101".ToBitArray();

            var newList = list.TakeLeft(4);
            
            Assert.AreEqual(newList.ToBitString(), "1111101");
        }

        [TestMethod]
        public void Test_Sub()
        {
            var list = "11001111101".ToBitArray();

            var newList = list.Sub(4, 3);

            Assert.AreEqual(newList.ToBitString(), "111");
        }

        [TestMethod]
        public void Test_Sub2()
        {
            var list = "11001111101".ToBitArray();

            var newList = list.Sub(4, 30);

            Assert.AreEqual(newList.ToBitString(), "1111101");
        }
    }
}
