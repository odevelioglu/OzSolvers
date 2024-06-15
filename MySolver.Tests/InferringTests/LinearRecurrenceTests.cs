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

        [Test]
        public void LinearTMP()
        {
            //var func = new Func<int, double>(d => Math.Log(MathHelper.Fact(d), 2));
            //var list = Enumerable.Range(0, 10).Select(n => func(n)).ToArray();

            //var list = new[] { 0d,0, 1, 8d/3d, 112d/24d, 872d/120d, 7344d/720d, 70560d/5040, 683904d/(5040*8) };

            //var func = new Func<int, double>(d => d*(d-1)/2d);
            //var list = Enumerable.Range(0, 20).Select(n => func(n)).ToArray();

            var list = new[] {
                0,
                2             / MathHelper.Fact(2), 
                16d           / MathHelper.Fact(3), 
                112d          / MathHelper.Fact(4), 
                872d          / MathHelper.Fact(5), 
                7512d         / MathHelper.Fact(6), 
                //71304d        / MathHelper.Fact(7),
                //741792d       / MathHelper.Fact(8), 
                //8409888d      / MathHelper.Fact(9), 
                //103331520d    / MathHelper.Fact(10)
            };
            //var list = new[] {0d, 0, 1, 3, 5, 9, 12, 17, 20 };

            var inferer = new LinearRecurrenceInferer();
            var res = inferer.Infer(list);
            var tmp = res.ToString();

            var tmp1 = Enumerable.Range(0, 10).Select(n => RecTmp(n)).ToArray();

            Assert.AreEqual("f(0) = 1 f(1) = 4 f(n) = 5 f(n - 1) + -6 f(n - 2)", res.ToString());
        }

        //f(0) = 0 f(1) = 0 f(2) = 1 f(n) = (8 / 3) f(n - 1) + -2.44444444444444 f(n - 2) + (181 / 135) f(n - 3)
        //private double RecTmp(int n)
        //{
        //    if (n == 0) return 0;
        //    if (n == 1) return 0;
        //    if (n == 2) return 1;

        //    return (8d/3d)* RecTmp(n - 1) 
        //        - 2.44444444444444d * RecTmp(n - 2) 
        //        + (181d / 135d) * RecTmp(n - 3);
        //}

        private double RecTmp(int n)
        {
            if (n == 0) return 0;
            if (n == 1) return 0;
            if (n == 2) return 1;

            return (8d / 3d) * RecTmp(n - 1)
                - 2.44444444444444d * RecTmp(n - 2)
                + (181d / 135d) * RecTmp(n - 3);
        }
    }
}
