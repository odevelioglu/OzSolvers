using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzCompress;

namespace Tests
{
    [TestClass]
    public class SearcherTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var list = "11001111101".ToBitArray();
            var toSearch = "1101".ToBitArray();

            var codon = Searcher.Search(list, toSearch, 0, 4);

            Assert.AreEqual(codon.Index, 7);
            Assert.AreEqual(codon.Length, 4);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var list = "100111110".ToBitArray();
            var toSearch = "1101".ToBitArray();
            
            var codon = Searcher.Search(list, toSearch, 0, 4);

            Assert.AreEqual(codon.Index, 6);
            Assert.AreEqual(codon.Length, 3);
        }

        [TestMethod]
        public void Test_StartIndex()
        {
            var list = "100111110".ToBitArray();
            var toSearch = "10".ToBitArray();

            var codon = Searcher.Search(list, toSearch, 1, 4);

            Assert.AreEqual(codon.Index, 7);
            Assert.AreEqual(codon.Length, 2);
        }

        [TestMethod]
        public void Test_Shift()
        {
            var list = "1001100100110".ToBitArray();
            var toSearch = "110110".ToBitArray();

            var shift = Searcher.SearchShift(
                list, toSearch, 0, 4);

            Assert.AreEqual(shift, 2);
        }
    }
}
