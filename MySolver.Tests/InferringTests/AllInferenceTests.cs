using System;
using System.Linq;
using MySolver.Inferring;
using NUnit.Framework;


namespace MySolver.Tests.InferringTests
{
    [TestFixture]
    public class AllInferenceTests
    {
        PolinomialInferer polinomialInferer = new PolinomialInferer();
        DifferenceTableInferer diffInferer = new DifferenceTableInferer();
        LinearRecurrenceInferer recurrenceInferer = new LinearRecurrenceInferer();

        [Test]
        public void InfereranceTest1()
        {
            var list = new[]{ 3, 5, 7, 9, 11 };
            //var func = new Func<int, double>(n => 3 + 2 * n);
            
            var poli = this.polinomialInferer.Infer(list);
            Assert.AreEqual("3 + 2n", poli.ToString());
            
            var node = diffInferer.Infer(list);
            Assert.AreEqual("(3 + ((n - 1) * 2))", node.ToString());
            
            var recurrence = this.recurrenceInferer.Infer(list);
            Assert.AreEqual("f(0) = 3 f(n) = (5 / 3) f(n - 1)", recurrence.ToString());
        }

        [Test]
        public void InfereranceTest2()
        {
            var list = new[] { 0, 1, 3, 7, 15, 31 };
            
            var poli = this.polinomialInferer.Infer(list);
            Assert.IsNull(poli);

            var node = diffInferer.Infer(list);
            Assert.IsNull(node);

            var func = new Func<int, int>(n => (int)Math.Pow(2, n) + 1);
            list = Enumerable.Range(0, 6).Select(func).ToArray();
            var recurrence = this.recurrenceInferer.Infer(list);

            //this fails: 
            //REad https://www.math.upenn.edu/~wilf/gfology2.pdf p3
            Assert.AreEqual("f(0) = 0 f(1) = 1 f(n) = 3 f(n - 1) + -2 f(n - 2)", recurrence.ToString());
        }
    }
}
