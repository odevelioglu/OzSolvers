using Microsoft.SolverFoundation.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzAlgo;
using System.Collections.Generic;
using System.Diagnostics;
using OzAlgo.LoopSolver;

namespace AlgoSolverTests
{
     [TestClass]
    public class Arithmeric2LoopSolverTests
    {
        
        [TestMethod]
        public void Test_MSSolver()
        {
            // -44.3940 = a * 50.0 + b * 37.0 + tx
            // -45.3049 = a * 43.0 + b * 39.0 + tx
            // -44.9594 = a * 52.0 + b * 41.0 + tx
            var context = SolverContext.GetContext();
            var model = context.CreateModel();

            var a = new Decision(Domain.Real, "a");
            var b = new Decision(Domain.Real, "b");
            var c = new Decision(Domain.Real, "c");
            model.AddDecisions(a, b, c);
            model.AddConstraint("eqA", -44.3940 == 50 * a + 37 * b + c);
            model.AddConstraint("eqB", -45.3049 == 43 * a + 39 * b + c);
            model.AddConstraint("eqC", -44.9594 == 52 * a + 41 * b + c);
            var solution = context.Solve();
            var results = solution.GetReport().ToString();
            Debug.WriteLine(results);
        }

        [TestMethod]
        public void Test_2()
        {
            var context = SolverContext.GetContext();
            var model = context.CreateModel();

            var k = new Decision(Domain.Integer, "k");
            var d = new Decision(Domain.Integer, "d");
            var kp = new Decision(Domain.Integer, "kp");
            var dp = new Decision(Domain.Integer, "dp");
            
            model.AddDecisions(k, d, kp, dp);
            model.AddConstraint("eq1", 1 == k + d + kp + dp);
            model.AddConstraint("eq2", 2 == 2*k + 2*d + kp + dp);
            model.AddConstraint("eq3", 3 == 3*k + 3*d + kp + dp);
            model.AddConstraint("eq4", 1 == 2*k + d + 2*kp + dp);
            model.AddConstraint("eq5", 2 == 4*k + 2*d + 2*kp + dp);
            model.AddConstraint("eq6", 1 == 3*k + d + 3*kp + dp);
            var solution = context.Solve();
            var results = solution.GetReport().ToString();
            Debug.WriteLine(results);
        }

        [TestMethod]
        public void Test_3()
        {
            var solution = Arithmeric2LoopSolver.SolveLoop(new List<int>() { 1, 2, 3, 1, 2, 1 }, 3, -1, 4);
            var results = solution.GetReport().ToString();
            Debug.WriteLine(results);
        }

        [TestMethod]
        public void Test_Inequalities1()
        {
            var list = new List<int>() { 1, 2, 3, 1, 2, 1 };
            var solver = new Arithmeric2LoopSolver(list);
            var res = solver.Solve();

            Debug.WriteLine(res[0]);

            Assert.AreEqual(res.Count, 1);
            Assert.AreEqual(res[0].k, 0);
            Assert.AreEqual(res[0].d, 1);
            Assert.AreEqual(res[0].kp, 0);
            Assert.AreEqual(res[0].dp, 0);
            Assert.AreEqual(res[0].Criterion.En, 3);
            Assert.AreEqual(res[0].Criterion.kpp, -1);
            Assert.AreEqual(res[0].Criterion.dpp, 4);
        }

        [TestMethod]
        public void Test_Inequalities2()
        {
            var list = new List<int>() { 8, 11, 13, 18, 18, 25 };
            var solver = new Arithmeric2LoopSolver(list);
            var res = solver.Solve();

            Debug.WriteLine(res[0]);

            Assert.AreEqual(res.Count, 1);
            Assert.AreEqual(res[0].k, 2);
            Assert.AreEqual(res[0].d, 1);
            Assert.AreEqual(res[0].kp, 3);
            Assert.AreEqual(res[0].dp, 2);
            Assert.AreEqual(res[0].Criterion.En, 3);
            Assert.AreEqual(res[0].Criterion.kpp, 0);
            Assert.AreEqual(res[0].Criterion.dpp, 2);
        }

        [TestMethod]
        public void Test_Inequalities3()
        {
            var list = new List<int>() { 1, 4, 14, 46, 230-84 };
            var solver = new Arithmeric2LoopSolver(list);
            var res = solver.Solve();

            Debug.WriteLine(res[0]);

            Assert.AreEqual(res.Count, 1);
            Assert.AreEqual(res[0].k, 2);
            Assert.AreEqual(res[0].d, 1);
            Assert.AreEqual(res[0].kp, 3);
            Assert.AreEqual(res[0].dp, 2);
            Assert.AreEqual(res[0].Criterion.En, 3);
            Assert.AreEqual(res[0].Criterion.kpp, 0);
            Assert.AreEqual(res[0].Criterion.dpp, 2);
        }
    }
}
