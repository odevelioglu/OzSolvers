using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzAlgo.LoopSolver;
using System.Collections.Generic;
using System.Diagnostics;

namespace AlgoSolverTests
{
    [TestClass]
    public class Geometric1LoopSolverTests
    {
        [TestMethod]
        public void Test_SolveGeometric1()
        {
            var list = new List<int>() { 6, 12, 24, 48, 96 };
            var solver = new Geometric11LoopSolver(list);
            var eq = solver.Solve();

            Assert.AreEqual(eq.Count, 1);
            Assert.AreEqual(eq[0].En, 5);
            Assert.AreEqual(eq[0].k, 3);
            Assert.AreEqual(eq[0].r, 2);

            Debug.WriteLine("Analyed " + string.Join(",", list));
            Debug.WriteLine("Closed form: " + eq[0]); // f(n) = 3r^n  n<=5
        }
    }
}
