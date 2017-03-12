using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzAlgo.LoopSolver;

namespace AlgoSolverTests
{
    [TestClass]
    public class Arithmeric1LoopSolverTests
    {
        [TestMethod]
        public void Test_SolveArithmetic1()
        {
            var list = new List<int>() {1, 3, 5, 7, 9};
            var solver = new Arithmeric1LoopSolver(list);
            var eq = solver.Solve();
            
            Assert.AreEqual(eq.Count, 1);
            Assert.AreEqual(eq[0].Criterion.En, 5);
            Assert.AreEqual(eq[0].k, 2);
            Assert.AreEqual(eq[0].d, -1);

            Debug.WriteLine("Found closed form: " + eq[0]); // f(n) = 2n-1  n<5
        }

        [TestMethod]
        public void Test_SolveArithmetic2()
        {
            var list = new List<int>() { 21,29,37,45,53,61,69,77,85,93};
            var solver = new Arithmeric1LoopSolver(list);
            var eq = solver.Solve();

            Assert.AreEqual(eq.Count, 1);
            Assert.AreEqual(eq[0].k, 8);
            Assert.AreEqual(eq[0].d, 13);

            Debug.WriteLine("Found closed form: " + eq[0]);
        }

        [TestMethod]
        public void Test_SolveArithmetic3()
        {
            var list = new List<int>() { 3,2,1,0 };
            var solver = new Arithmeric1LoopSolver(list);
            var eq = solver.Solve();

            Assert.AreEqual(eq.Count, 1);
            Assert.AreEqual(eq[0].k, -1);
            Assert.AreEqual(eq[0].d, 4);

            Debug.WriteLine("Found closed form: " + eq[0]);
        }
    }
}
