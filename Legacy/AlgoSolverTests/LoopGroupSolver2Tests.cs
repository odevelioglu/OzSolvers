using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzAlgo.LoopSolver;

namespace AlgoSolverTests
{
    [TestClass]
    public class LoopGroupSolver2Tests
    {
        [TestMethod]
        public void Test4()
        {
            var solver = new LoopGroupSolver2();
            int count = 4;
            var results = solver.Solve(count);

            Assert.AreEqual(results.Count, 3);

            Assert.AreEqual(results[0].En, 2);
            Assert.AreEqual(results[0].kpp, 2);
            Assert.AreEqual(results[0].dpp, -1);

            Assert.AreEqual(results[1].En, 2);
            Assert.AreEqual(results[1].kpp, 0);
            Assert.AreEqual(results[1].dpp, 2);

            Assert.AreEqual(results[2].En, 2);
            Assert.AreEqual(results[2].kpp, -2);
            Assert.AreEqual(results[2].dpp, 5);

            Debug.WriteLine("Found {0} result for {1}:", results.Count, count);
            Debug.WriteLine("-------------------------");
            foreach (var eq in results)
            {
                Debug.WriteLine(eq + "\t\t\t" + eq.GetValuesStr());
            }
        }

        [TestMethod]
        public void Test6()
        {
            var solver = new LoopGroupSolver2();
            int count = 6;
            var results = solver.Solve(count);

            Assert.AreEqual(results.Count, 8);

            Assert.AreEqual(results[0].En, 2);
            Assert.AreEqual(results[0].kpp, 4);
            Assert.AreEqual(results[0].dpp, -3);

            Assert.AreEqual(results[1].En, 2);
            Assert.AreEqual(results[1].kpp, 2);
            Assert.AreEqual(results[1].dpp, 0);

            Debug.WriteLine("Found {0} result for {1}:", results.Count, count);
            Debug.WriteLine("-------------------------");
            foreach (var eq in results)
            {
                Debug.WriteLine(eq + "\t\t\t" + eq.GetValuesStr());
            }
        }

    }
}
