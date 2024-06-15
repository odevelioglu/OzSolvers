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

        [Test]
        public void PolinomialInfererDegreeTMP()
        {
            //var a = new[] { 0d, 0, 1, 3, 6, 10, 15, 21, 28 };
            //var a = new[] { 0d, 0, 1, 3, 5, 9, 12, 17, 20};
            //var a = new [] { 0, 1, 2.66, 4.66, 7.26 };
            //var a = new[] {0, 0, 1, 2.58, 4.58 };

            //var a = new[] { 0, 1, 3, 6d};

            //var a = new[] { 0, 0.5d, 1.5d, /*3d, 5d, 7.5d, 10.5d*/ };

            //var a = new[] {
            //    0,
            //    2          / MathHelper.Fact(2),
            //    16d        / MathHelper.Fact(3),
            //    //112d       / MathHelper.Fact(4),
            //    //872d       / MathHelper.Fact(5),
            //    //7512d      / MathHelper.Fact(6),
            //    //71304d     / MathHelper.Fact(7),
            //    //741792d    / MathHelper.Fact(8),
            //    //8409888d   / MathHelper.Fact(9),
            //    //103331520d / MathHelper.Fact(10)
            //};

            var a = new double[] 
            {
0      ,
0      ,
1      ,
2.645  ,
4.671  ,
7.244  ,
10.476 ,
14.069 ,
18.399 ,
23.086 ,
28.389 ,
34.21  ,
40.534 ,
47.803 ,
54.849 ,
62.292 ,
71.132 ,
79.791 ,
89.115 ,
            };

            var inferer = new PolinomialInferer();
            var poli = inferer.InternalInfer(a);

            
            var tmp = Enumerable.Range(0, 19).Select(poli.Eval).ToArray();
            var tmp2 = Enumerable.Range(0, 19).Select(x=>x*x/4).ToArray();
            var tmp3 = tmp.Select((x,i) => tmp2[i] / x).ToArray();

            Assert.AreEqual("1 + 2n + 3n^2", poli.ToString());

            
        }
    }
}
