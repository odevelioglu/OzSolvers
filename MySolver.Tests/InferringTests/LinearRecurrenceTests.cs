namespace MySolver.Tests.InferringTests
{
    using System;
    using System.Linq;

    using MySolver.Inferring;
    using NUnit.Framework;

    [TestFixture]
    public class LinearRecurrenceTests
    {
        [Test]
        public void Linear()
        {
            var tmp1 = Enumerable.Range(0, 10).Select(n=>Rec(n)).ToArray();
            var tmp2 = Enumerable.Range(0, 10)
                .Select(n => 2*Math.Pow(3,n) - Math.Pow(2,n))
                .ToArray();

            var tmp3 = Enumerable.Range(0, 10).Select(n => Rec2(n));
            // f(0) = 0
            // f(1) = 4
            // f(n) = 5f(n - 1) - 6f(n - 2)
            // closed form: (2 * 3^n) - 2^n
            var list = new[] { 1d, 4, 14, 46, 146, 454};

            var inferer = new LinearRecurrenceInferer();
            var res = inferer.Infer(list);
            var tmp = res.ToString();

            Assert.AreEqual("f(0) = 1 f(1) = 4 f(n) = 5 f(n - 1) + -6 f(n - 2)", res.ToString());
        }

        private double Rec(int n)
        {
            if (n == 0) return 1;
            if (n == 1) return 4;
            return 5 * Rec(n - 1) - 6 * Rec(n - 2);
        }

        private double Rec2(int n) // n>0
        {
            if (n == 0) return 1;
            return 3 * Rec2(n - 1) + Math.Pow(2, n - 1) ;
        }
    }
}
