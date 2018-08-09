using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using log4net.Config;
using System.Diagnostics;
using NUnit.Framework;

namespace MySolver.Tests
{
    [TestFixture]
    public class SearcherTests
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Searcher));

        private Stopwatch watch = new Stopwatch();
        
        [OneTimeSetUp]
        public void Init()
        {
            //if(File.Exists("Log.log"))
            //    File.Delete("Log.log");
            XmlConfigurator.Configure();
            //Log.Info("Start");

            watch.Start();
        }

        [Test]
        public void TestMethod1()
        {
            //1+2+3+...+n= 
            var funcs = new List<BinaryFunction> { Commons.mul, Commons.add, Commons.div, Commons.sub, Commons.pow, Commons.log };
            var constants = new List<Constant> { Commons.c1, Commons.c2 };
            var toSolve = new Func<int[], double>(n => Enumerable.Range(1, n[0]).Sum());
            var variables = new List<VariableInfo> { new VariableInfo("n", new[] { 1, 2, 3, 4 }) };
            var searcher = new Searcher(toSolve, variables, 5, funcs, constants);
            searcher.StopAtFirstResult = true;

            searcher.Search();

            Console.WriteLine("NumberOfVerifiedSolutions: " + searcher.NumberOfVerifiedSolutions);
            Console.WriteLine("Elapsed time: " + watch.ElapsedMilliseconds);
            Assert.AreEqual("((n / 2) * (n + 1))", searcher.FinalResults[0].ToString());
        }

        [Test]
        public void TestMethod2()
        {
            // 1+3+5+...2n-1
            var funcs = new List<BinaryFunction> { Commons.mul, Commons.add, Commons.div, Commons.sub, Commons.pow, Commons.log };
            var constants = new List<Constant> { Commons.c1, Commons.c2 };
            var toSolve = new Func<int[], double>(n => Enumerable.Range(1, 2 * n[0] - 1).Where(f => f % 2 == 1).Sum());
            var variables = new List<VariableInfo> { new VariableInfo("n", new[] { 1, 2, 3, 4 }) };

            var searcher = new Searcher(toSolve, variables, 5, funcs, constants);
            searcher.StopAtFirstResult = true;

            searcher.Search();

            Console.WriteLine("NumberOfVerifiedSolutions: " + searcher.NumberOfVerifiedSolutions);
            Console.WriteLine("Elapsed time: " + watch.ElapsedMilliseconds);
            Assert.AreEqual("(n * n)", searcher.FinalResults[0].ToString());
        }

        [Test]
        public void TestMethod3()
        {
            // 2+4+6...+2n=
            var funcs = new List<BinaryFunction> { Commons.mul, Commons.add, Commons.div, Commons.sub, Commons.pow, Commons.log };
            var constants = new List<Constant> { Commons.c1, Commons.c2 };
            var toSolve = new Func<int[], double>(n => Enumerable.Range(1, 2 * n[0]).Where(f => f % 2 == 0).Sum());
            var variables = new List<VariableInfo> { new VariableInfo("n", new[] { 1, 2, 3, 4 }) };

            var searcher = new Searcher(toSolve, variables, 5, funcs, constants);
            searcher.StopAtFirstResult = true;

            searcher.Search();

            Console.WriteLine("NumberOfVerifiedSolutions: " + searcher.NumberOfVerifiedSolutions);
            Console.WriteLine("Elapsed time: " + watch.ElapsedMilliseconds);
            Assert.AreEqual("(n * (n + 1))", searcher.FinalResults[0].ToString());
        }

        [Test]
        public void TestMethod4()
        {
            //SERIE GEOMETRIQUE
            // 1+q+q^2...+q^n= (1-q^(n+1))/1-q

            var funcs = new List<BinaryFunction> { Commons.div, Commons.sub, Commons.pow, Commons.add, Commons.mul, Commons.log, Commons.neg};
            var constants = new List<Constant> { Commons.c1, Commons.c2 };
            var toSolve = new Func<int[], double>(vars =>
            {
                double sum = 0;
                for (int n = 0; n <= vars[0]; n++)
                {
                    sum += Math.Pow(vars[1], n);
                }
                return sum;
            });

            var variables = new List<VariableInfo>
            {
                new VariableInfo("n", new[] { 2, 3, 5 }),
                new VariableInfo("q", new[] { 13, 29 })
            };

            var searcher = new Searcher(toSolve, variables, 4, funcs, constants);
            searcher.StopAtFirstResult = true;

            searcher.Search();
            Console.WriteLine("NumberOfVerifiedSolutions: " + searcher.NumberOfVerifiedSolutions);
            Console.WriteLine("Elapsed time: " + watch.ElapsedMilliseconds);

            Assert.AreEqual("(((1 / q) - (q ^ n)) / ((1 - q) / q))", searcher.FinalResults[0].ToString());
        }

        [Test]
        public void TestMethod5()
        {
            // (1 + a) ^ n = Sum(k: 0..n) C(k,n) a^k

            var funcs = new List<BinaryFunction> { Commons.div, Commons.sub, Commons.pow, Commons.add, Commons.mul, Commons.log };
            var constants = new List<Constant> { Commons.c1, Commons.c2 };
            var toSolve = new Func<int[], double>(vars =>
            {
                double sum = 0;
                for (int k = 0; k <= vars[0]; k++)
                {
                    sum += MathHelper.Combin(vars[0], k) * Math.Pow(vars[1], k);
                }
                return sum;
            });

            var variables = new List<VariableInfo>
            {
                new VariableInfo("n", new[] { 2, 3, 5 }),
                new VariableInfo("q", new[] { 13, 29 })
            };

            var searcher = new Searcher(toSolve, variables, 3, funcs, constants);
            searcher.StopAtFirstResult = true;

            searcher.Search();

            Console.WriteLine("NumberOfVerifiedSolutions: " + searcher.NumberOfVerifiedSolutions);
            Console.WriteLine("Elapsed time: " + watch.ElapsedMilliseconds);

            Assert.AreEqual("((1 + q) ^ n)", searcher.FinalResults[0].ToString());
        }

        [Test]
        public void TestMethod6()
        {
            // (-1)^n / n => n-> inf = 

            var funcs = new List<BinaryFunction> { Commons.div, Commons.sub, Commons.pow, Commons.add, Commons.mul, Commons.log, Commons.neg };
            var constants = new List<Constant> { Commons.c1, Commons.c2 };
            var toSolve = new Func<int[], double>(
                vars =>
                    {
                        double sum = 0;
                        for (int n = 1; n <= vars[0]; n++)
                        {
                            sum += Math.Pow(-1, n) / n;
                        }
                        return sum;
                    });

            var variables = new List<VariableInfo> { new VariableInfo("n", new[] { 20000 }) };

            var searcher = new Searcher(toSolve, variables, 3, funcs, constants);
            searcher.StopAtFirstResult = true;

            searcher.Search();

            Console.WriteLine("NumberOfVerifiedSolutions: " + searcher.NumberOfVerifiedSolutions);
            Console.WriteLine("Elapsed time: " + watch.ElapsedMilliseconds);

            Assert.AreEqual("(log(2) / -(1))", searcher.FinalResults[0].ToString());
        }

        [Test]        
        [Ignore("")]
        public void TestMethodPi()
        {
            // n:0..inf (-1)^n / (2*n + 1) = Pi / 4

            var funcs = new List<BinaryFunction> { Commons.div, Commons.pow, Commons.add, Commons.mul, Commons.neg };
            var constants = new List<Constant> { Commons.c1, Commons.c2 };
            var toSolve = new Func<int[], double>(
                vars =>
                {
                    double sum = 0;
                    for (int n = 0; n <= vars[0]; n++)
                    {
                        sum += Math.Pow(-1, n) / (2 * n + 1);
                    }
                    return sum;

                    //return Math.PI / 4;
                });

            //var tmp = toSolve(new[] { 100000 }) * 4 / Math.PI;

            var variables = new List<VariableInfo> { new VariableInfo("n", new[] { 100000 }) };

            var searcher = new Searcher(toSolve, variables, 4, funcs, constants);
            searcher.StopAtFirstResult = true;

            searcher.Search();

            Console.WriteLine("NumberOfVerifiedSolutions: " + searcher.NumberOfVerifiedSolutions);
            Console.WriteLine("Elapsed time: " + watch.ElapsedMilliseconds);

            Assert.AreEqual("", searcher.FinalResults[0].ToString());
        }

        [Test]
        [Ignore("")]
        public void TestMethodSemantic()
        {
            var funcs = new List<BinaryFunction> { Commons.div, Commons.sub, };
            var constants = new List<Constant> { Commons.c1, };
            var toSolve = new Func<int[], double>(n => Enumerable.Range(1, n[0]).Sum());

            var variables = new List<VariableInfo>
            {
                new VariableInfo("n", new[] { 3, 5, 7 }),
                //new VariableInfo("q", new[] { 11, 17, 29 })
            };

            var searcher = new Searcher(toSolve, variables, 3, funcs, constants);
            //searcher.StopAtFirstResult = true;

            searcher.Search();
            Console.WriteLine("NumberOfVerifiedSolutions: " + searcher.NumberOfVerifiedSolutions);
            Console.WriteLine("Elapsed time: " + watch.ElapsedMilliseconds);

            Assert.AreEqual("(((1 / q) - (q ^ n)) / ((1 - q) * (1 / q)))", searcher.FinalResults[0].ToString());
        }

        [Test]
        public void TestSemEq()
        {
            var leftcons = new Node(Commons.c1, null, null);
            var rightcons = new Node(Commons.c2, null, null);
            var root = new Node(Commons.add, leftcons, rightcons);

            var root2 = new Node(Commons.add, new Node(Commons.c1, null, null), new Node(Commons.c2, null, null));

            Assert.IsTrue(leftcons.SemEq(leftcons));
            Assert.IsFalse(leftcons.SemEq(rightcons));
            Assert.IsFalse(root.SemEq(leftcons));
            Assert.IsTrue(root.SemEq(root));
            Assert.IsFalse(root.SemEq(leftcons));
            Assert.IsTrue(root.SemEq(root2));
        }
        
        [Test]
        public void TestPermut()
        {
            var queue = new Queue<int[]>();
            queue.Enqueue(new[] { 1, 2 });
            queue.Enqueue(new[] { 3, 4, 8 });
            queue.Enqueue(new[] { 5, 6 });

            var permuted = MathHelper.Permut(queue).ToList();

            Assert.AreEqual(permuted.Count, 12);
        }

        [Test]
        public void TestMethodCatalanNumbers()
        {
            // C(n) = (2n)!/(n!(n+1)!). 

            var funcs = new List<BinaryFunction> { Commons.div, Commons.add, Commons.mul, Commons.fac };
            var constants = new List<Constant> { Commons.c1, Commons.c2 };
            var toSolve = new Func<int[], double>(vars =>
            {
                var n = vars[0];
                return MathHelper.Fact(2 * n) / (MathHelper.Fact(n) * MathHelper.Fact(n + 1));
            });

            var variables = new List<VariableInfo>
            {
                new VariableInfo("n", new[] { 1, 2, 3, 4, 5, 6 }),
            };

            var searcher = new Searcher(toSolve, variables, 5, funcs, constants);
            searcher.StopAtFirstResult = true;

            searcher.Search();

            Console.WriteLine("NumberOfVerifiedSolutions: " + searcher.NumberOfVerifiedSolutions);
            Console.WriteLine("Elapsed time: " + watch.ElapsedMilliseconds);

            //Assert.AreEqual("(2n)! / ((n)! * ((n + 1))!)", searcher.FinalResults[0].ToString());
        }


    }
}
