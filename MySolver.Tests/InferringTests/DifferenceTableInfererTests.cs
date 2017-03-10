using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySolver.Inferring;

namespace MySolver.Tests
{
    [TestClass]
    public class DifferenceTableInfererTests
    {
        DifferenceTableInferer inferer = new DifferenceTableInferer();

        [TestMethod]
        public void TestMethod1()
        {
            //https://www.algebra.com/algebra/homework/Sequences-and-series/Sequences-and-series.faq.question.155130.html
            var sequence = new[] { 9, 73, 241, 561, 1081, 1849 };
            var node = inferer.Infer(sequence);

            var nextElem = node.Eval(sequence.Length + 1);
            Assert.AreEqual(2913, nextElem);
            Assert.AreEqual("(((((9 + (((n - 1) / (1)!) * 64)) + ((((n - 1) * (n - 2)) / (2)!) * 104)) + (((((n - 1) * (n - 2)) * (n - 3)) / (3)!) * 48)) + ((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) / (4)!) * 0)) + (((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) * (n - 5)) / (5)!) * 0))", node.ToString());
        }

        [TestMethod]
        public void TestMethod2()
        {
            var sequence = new[] { 1, 2, 3, 1, 2, 1 };
            var node = inferer.Infer(sequence);

            var nextElem = node.Eval(sequence.Length + 1);
            Assert.AreEqual(-38, nextElem); // :)
            Assert.AreEqual("(((((1 + (((n - 1) / (1)!) * 1)) + ((((n - 1) * (n - 2)) / (2)!) * 0)) + (((((n - 1) * (n - 2)) * (n - 3)) / (3)!) * -3)) + ((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) / (4)!) * 9)) + (((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) * (n - 5)) / (5)!) * -20))", node.ToString());
        }

        [TestMethod]
        public void TestMethodCatalan()
        {
            // =(2n)!/(n!(n+1)!)
            // 1, 1, 2, 5, 14, 42, 132, 429, 1430, 4862, 16796, 58786
            var sequence = new[] { 1, 1, 2, 5, 14, 42, 132, 429 };
            var node = inferer.Infer(sequence);

            var simplified = Simplifier.Simplify(node);

            var nextElem = node.Eval(sequence.Length + 1);
            //Assert.AreEqual(-38, nextElem); // :)
            Assert.AreEqual("(((((1 + (((n - 1) / (1)!) * 1)) + ((((n - 1) * (n - 2)) / (2)!) * 0)) + (((((n - 1) * (n - 2)) * (n - 3)) / (3)!) * -3)) + ((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) / (4)!) * 9)) + (((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) * (n - 5)) / (5)!) * -20))", node.ToString());
        }

        [TestMethod]
        public void TestMethod3()
        {
            //1+2+3+...+n= 
            var toSolve = new Func<int, double>(n => Enumerable.Range(1, n).Sum());
            var sequence = Enumerable.Range(1, 10).Select(n => (int)toSolve(n)).ToArray(); // 1, 3, 6, 10, 15, 21, 28
            var node = inferer.Infer(sequence);
            
            Assert.AreEqual("(((((((((1 + (((n - 1) / (1)!) * 2)) + ((((n - 1) * (n - 2)) / (2)!) * 1)) + (((((n - 1) * (n - 2)) * (n - 3)) / (3)!) * 0)) + ((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) / (4)!) * 0)) + (((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) * (n - 5)) / (5)!) * 0)) + ((((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) * (n - 5)) * (n - 6)) / (6)!) * 0)) + (((((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) * (n - 5)) * (n - 6)) * (n - 7)) / (7)!) * 0)) + ((((((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) * (n - 5)) * (n - 6)) * (n - 7)) * (n - 8)) / (8)!) * 0)) + (((((((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) * (n - 5)) * (n - 6)) * (n - 7)) * (n - 8)) * (n - 9)) / (9)!) * 0))", node.ToString());
        }
    }
}
