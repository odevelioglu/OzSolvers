using System;
using System.Linq;
using MySolver.Inferring;
using NUnit.Framework;

namespace MySolver.Tests.InferringTests
{
    [TestFixture]
    public class PolinomialInfererTests
    {
        [Test]
        public void PolinomialInfererDegree0()
        {
            var inferer = new PolinomialInferer();
            var poli = inferer.InternalInfer(new[] { 7d });

            Assert.AreEqual("7", poli.ToString());
        }

        [Test]
        public void PolinomialInfererDegree1()
        {
            var func = new Func<int, double>(n => 3 + 2*n);
            var a = Enumerable.Range(0, 2).Select(func).ToArray();

            var inferer = new PolinomialInferer();
            var poli = inferer.InternalInfer(a);

            Assert.AreEqual("3 + 2n", poli.ToString());
        }

        [Test]
        public void PolinomialInfererDegree2()
        {
            var func = new Func<int, double>(n => 1 + 2 * n + 3 * Math.Pow(n, 2));
            var a = Enumerable.Range(0, 3).Select(func).ToArray(); // you need degree of func+1 elems

            var inferer = new PolinomialInferer();
            var poli = inferer.InternalInfer(a);

            Assert.AreEqual("1 + 2n + 3n^2", poli.ToString());
        }

        [Test]
        public void PolinomialInfererDegree2Series()
        {
            // 1+3+5+...2n-1 = n^2
            var func = new Func<int, double>(n =>
            {
                var sum = 0;
                for (var i = 0; i <= 2 * n - 1; i++)
                {
                    if (i % 2 == 1)
                        sum += i;
                }
                return sum;
            });
            
            var a = Enumerable.Range(0, 3).Select(func).ToArray(); 

            var inferer = new PolinomialInferer();
            var poli = inferer.InternalInfer(a);

            Assert.AreEqual("n^2", poli.ToString());
        }
    }
}
