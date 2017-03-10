using System;
using MySolver.Inferring;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace MySolver.Tests.InferringTests
{
    [TestFixture]
    public class RationalInfererTests
    {
        RationalInferer inferer = new RationalInferer();

        [Test]
        public void RationalInfererAll()
        {
            //1 / 503 
            var res = inferer.Infer(new[] { 0, 0, 0, 1, 9, 8, 8, 0, 7, 1, 5, 7, 0, 5, 7, 6, 5, 4, 0, 7, 5, 5, 4, 6, 7, 1, 9, 6, 8, 1, 9, 0, 8, 5, 4, 8, 7, 0, 7, 7, 5, 3, 4, 7, 9, 1, 2, 5, 2, 4, 8, 5, 0, 8, 9, 4, 6, 3, 2, 2, 0, 6, 7, 5, 9, 4, 4, 3, 3, 3, 9, 9, 6, 0, 2, 3, 8, 5, 6, 8, 5, 8, 8, 4, 6, 9, 1, 8, 4, 8, 9, 0, 6, 5, 6, 0, 6, 3, 6, 1, 8, 2, 9, 0, 2, 5, 8, 4, 4, 9, 3, 0, 4, 1, 7, 4, 9, 5, 0, 2, 9, 8, 2, 1, 0, 7, 3, 5, 5, 8, 6, 4, 8, 1, 1, 1, 3, 3, 2, 0, 0, 7, 9, 5, 2, 2, 8, 6, 2, 8, 2, 3, 0, 6, 1, 6, 3, 0, 2, 1, 8, 6, 8, 7, 8, 7, 2, 7, 6, 3, 4, 1, 9, 4, 8, 3, 1, 0, 1, 3, 9, 1, 6, 5, 0, 0, 9, 9, 4, 0, 3, 5, 7, 8, 5, 2, 8, 8, 2, 7, 0, 3, 7, 7, 7, 3, 3, 5, 9, 8, 4, 0, 9, 5, 4, 2, 7, 4, 3, 5, 3, 8, 7, 6, 7, 3, 9, 5, 6, 2, 6, 2, 4, 2, 5, 4, 4, 7, 3, 1, 6, 1, 0, 3, 3, 7, 9, 7, 2, 1, 6, 6, 9, 9, 8, 0, 1, 1, 9, 2, 8, 4, 2, 9, 4, 2, 3, 4, 5, 9, 2, 4, 4, 5, 3, 2, 8, 0, 3, 1, 8, 0, 9, 1, 4, 5, 1, 2, 9, 2, 2, 4, 6, 5, 2, 0, 8, 7, 4, 7, 5, 1, 4, 9, 1, 0, 5, 3, 6, 7, 7, 9, 3, 2, 4, 0, 5, 5, 6, 6, 6, 0, 0, 3, 9, 7, 6, 1, 4, 3, 1, 4, 1, 1, 5, 3, 0, 8, 1, 5, 1, 0, 9, 3, 4, 3, 9, 3, 6, 3, 8, 1, 7, 0, 9, 7, 4, 1, 5, 5, 0, 6, 9, 5, 8, 2, 5, 0, 4, 9, 7, 0, 1, 7, 8, 9, 2, 6, 4, 4, 1, 3, 5, 1, 8, 8, 8, 6, 6, 7, 9, 9, 2, 0, 4, 7, 7, 1, 3, 7, 1, 7, 6, 9, 3, 8, 3, 6, 9, 7, 8, 1, 3, 1, 2, 1, 2, 7, 2, 3, 6, 5, 8, 0, 5, 1, 6, 8, 9, 8, 6, 0, 8, 3, 4, 9, 9, 0, 0, 5, 9, 6, 4, 2, 1, 4, 7, 1, 1, 7, 2, 9, 6, 2, 2, 2, 6, 6, 4, 0, 1, 5, 9, 0, 4, 5, 7, 2, 5, 6, 4, 6, 1, 2, 3, 2, 6, 0, 4, 3, 7, 3, 7, 5, 7, 4, 5, 5, 2, 6, 8, 3, 8, 9, 6, 6, 2, 0, 2, 7, 8, 3, 3 });

            Assert.AreEqual(new Tuple<long, long>(1, 503), res);
        }

        [Test]
        public void RationalInfererFromPart()
        {
            var res = inferer.InferFromPart(new[] { 4, 3, 3, 3, 9, 9 });
            Assert.AreEqual(new Tuple<long, long>(218, 503), res);
        }

        [Test]
        public void RationalInferer1()
        {
            var res = inferer.InferFromPart(new[] { 0, 0, 1, 9, 8, 8 });
            Assert.AreEqual(new Tuple<long, long>(1, 503), res);
        }

        [Test]
        public void RationalInfererAllParts()
        {
            //1 / 503 
            var list = new[] { 0, 0, 0, 1, 9, 8, 8, 0, 7, 1, 5, 7, 0, 5, 7, 6, 5, 4, 0, 7, 5, 5, 4, 6, 7, 1, 9, 6, 8, 1, 9, 0, 8, 5, 4, 8, 7, 0, 7, 7, 5, 3, 4, 7, 9, 1, 2, 5, 2, 4, 8, 5, 0, 8, 9, 4, 6, 3, 2, 2, 0, 6, 7, 5, 9, 4, 4, 3, 3, 3, 9, 9, 6, 0, 2, 3, 8, 5, 6, 8, 5, 8, 8, 4, 6, 9, 1, 8, 4, 8, 9, 0, 6, 5, 6, 0, 6, 3, 6, 1, 8, 2, 9, 0, 2, 5, 8, 4, 4, 9, 3, 0, 4, 1, 7, 4, 9, 5, 0, 2, 9, 8, 2, 1, 0, 7, 3, 5, 5, 8, 6, 4, 8, 1, 1, 1, 3, 3, 2, 0, 0, 7, 9, 5, 2, 2, 8, 6, 2, 8, 2, 3, 0, 6, 1, 6, 3, 0, 2, 1, 8, 6, 8, 7, 8, 7, 2, 7, 6, 3, 4, 1, 9, 4, 8, 3, 1, 0, 1, 3, 9, 1, 6, 5, 0, 0, 9, 9, 4, 0, 3, 5, 7, 8, 5, 2, 8, 8, 2, 7, 0, 3, 7, 7, 7, 3, 3, 5, 9, 8, 4, 0, 9, 5, 4, 2, 7, 4, 3, 5, 3, 8, 7, 6, 7, 3, 9, 5, 6, 2, 6, 2, 4, 2, 5, 4, 4, 7, 3, 1, 6, 1, 0, 3, 3, 7, 9, 7, 2, 1, 6, 6, 9, 9, 8, 0, 1, 1, 9, 2, 8, 4, 2, 9, 4, 2, 3, 4, 5, 9, 2, 4, 4, 5, 3, 2, 8, 0, 3, 1, 8, 0, 9, 1, 4, 5, 1, 2, 9, 2, 2, 4, 6, 5, 2, 0, 8, 7, 4, 7, 5, 1, 4, 9, 1, 0, 5, 3, 6, 7, 7, 9, 3, 2, 4, 0, 5, 5, 6, 6, 6, 0, 0, 3, 9, 7, 6, 1, 4, 3, 1, 4, 1, 1, 5, 3, 0, 8, 1, 5, 1, 0, 9, 3, 4, 3, 9, 3, 6, 3, 8, 1, 7, 0, 9, 7, 4, 1, 5, 5, 0, 6, 9, 5, 8, 2, 5, 0, 4, 9, 7, 0, 1, 7, 8, 9, 2, 6, 4, 4, 1, 3, 5, 1, 8, 8, 8, 6, 6, 7, 9, 9, 2, 0, 4, 7, 7, 1, 3, 7, 1, 7, 6, 9, 3, 8, 3, 6, 9, 7, 8, 1, 3, 1, 2, 1, 2, 7, 2, 3, 6, 5, 8, 0, 5, 1, 6, 8, 9, 8, 6, 0, 8, 3, 4, 9, 9, 0, 0, 5, 9, 6, 4, 2, 1, 4, 7, 1, 1, 7, 2, 9, 6, 2, 2, 2, 6, 6, 4, 0, 1, 5, 9, 0, 4, 5, 7, 2, 5, 6, 4, 6, 1, 2, 3, 2, 6, 0, 4, 3, 7, 3, 7, 5, 7, 4, 5, 5, 2, 6, 8, 3, 8, 9, 6, 6, 2, 0, 2, 7, 8, 3, 3 };

            for (var i = 1; i < list.Length - 5; i++)
            {
                var res = inferer.InferFromPart(list.Sub(i, 6));

                Assert.AreEqual(503, res.Item2);
            }
        }

        [Test]
        public void RationalInferer1Over7()
        {
            var res = inferer.Infer(new[] { 0, 1, 4, 2, 8, 5, 7 });
            Assert.AreEqual(new Tuple<long, long>(1, 7), res);
        }

        [Test]
        public void RationalInferer3Over7()
        {
            var res = inferer.Infer(new[] { 0, 4, 2, 8, 5, 7, 1 });
            Assert.AreEqual(new Tuple<long, long>(3, 7), res);
        }

        [TestCase(new[] { 1 }, 1, 10)]
        [TestCase(new[] { 1, 2, 1 }, 4, 33)]
        [TestCase(new[] { 1, 2, 3, 1, 2, 1 }, 3309, 26876)]
        [TestCase(new[] { 1, 2, 3, 4, 1, 2, 3, 1, 2, 1 }, 16100, 130457)]
        [TestCase(new[] { 1, 2, 3, 4, 5, 1, 2, 3, 4, 1, 2, 3, 1, 2, 1 }, 56454119, 457298944)]
        public void MyCoolNumbers(int[] list, long num, long denum)
        {
            var res = inferer.InferFromPart(list);
            Assert.AreEqual(new Tuple<long, long>(num, denum), res);
        }

        [TestCase("4, 2, 6, 7", 415, 93)]
        [TestCase("0, 503", 1, 503)]
        [TestCase("0, 1, 5, 2, 2", 27, 32)]
        public void ContinuedFractionDivision(string expected, int divisor, int divident)
        {
            var fraction = ContinuedFraction.FromDecimal((decimal)divisor / divident);
            Assert.AreEqual(expected, fraction.ToString());
        }

        [Test]
        public void ContinuedFractionDecimal()
        {
            var fraction = ContinuedFraction.FromDecimal(3.245m);
            Assert.AreEqual("3, 4, 12, 4", fraction.ToString());
        }

        [Test]
        public void ConvergentCalculator()
        {
            var calculator = new ConvergentCalculator();
            calculator.Add(new long[] { 0, 1, 5, 2, 2 });
            
            Assert.AreEqual(27, calculator.R);
            Assert.AreEqual(32, calculator.P);
        }
       
    }
}
