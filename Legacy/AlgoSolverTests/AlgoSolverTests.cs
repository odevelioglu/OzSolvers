using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzAlgo;
using OzAlgo.LoopSolver;
using System.Collections.Generic;
using System.Diagnostics;

namespace AlgoSolverTests
{
    [TestClass]
    public class AlgoSolverTests
    {
        [TestMethod]
        public void OrderedListEqualityComparer_Test()
        {
            var set = new HashSet<List<int>>(new OrderedListEqualityComparer()) { new List<int>() { 1, 3, 5} };
            
            Assert.IsTrue(set.Contains(new List<int>(){1, 3, 5}));
        }

        //[TestMethod]
        //public void Isreducible_Test()
        //{
        //    var solver = new SortSolver { List = new List<int>() { 9, 4, 2 } };
        //    Assert.IsTrue(solver.IsReducible(new List<int>() {1, 1}));

        //    solver = new SortSolver { List = new List<int>() { 9, 4, 2, 1 } };
        //    Assert.IsTrue(solver.IsReducible(new List<int>() { 1, 0, 1,0, 1,0 }));
        //}

        [TestMethod]
        public void TestMethod_Solve3()
        {
            var solver = new SortSolver(new List<int>() {9, 4, 2});

            solver.Solve();

            FunTree.VisitToBottom(solver.Root, node => Debug.WriteLine(node));
            
            Debug.WriteLine("");

            foreach (var node in solver.ResultNodes)
            {
                Debug.WriteLine("Result: " + node);
            }
        }

        [TestMethod]
        public void Test_ListExtensions()
        {
            var l1 = new List<int>() { 1, 2 };
            var l2 = new List<int>() { 2, 1 };
            
            Assert.IsTrue(l1.HasSameElements(l2));
            
            var list1 = new List<List<int>>()
            {
                new List<int>{0,1},
                new List<int>{1,2},
                new List<int>{2,3}
            };

            var list2 = new List<List<int>>()
            {
                new List<int>{2,3},
                new List<int>(){1,2},
                new List<int>(){0,1}
            };

            Assert.IsTrue(list1.HasSameElements(list2));

            Assert.IsTrue(list1.IsReverse(list2));
        }

        [TestMethod]
        public void TestMethod_Solve4()
        {
            var watch = new Stopwatch();
            watch.Start();

            var genSolver = new IterativeGeneralizationSolver("N");

            //Solve(new List<int>() );
            var solutions = Solve(new List<int>() { 9 });
            //genSolver.Solve(1, solutions);
            //solutions = Solve(new List<int>() { 9, 5 });
            //genSolver.Solve(2, solutions);
            solutions = Solve(new List<int>() { 9, 5, 4 });
            genSolver.Solve(2, solutions);
            solutions = Solve(new List<int>() { 9, 5, 4, 2 });
            genSolver.Solve(2, solutions);
            
            
            Solve(new List<int>() { 9, 5, 4, 2, 1 });
            
            watch.Stop();
            Debug.WriteLine("Overall time: {0}ms", watch.ElapsedMilliseconds);
        }

        private static List<LoopEquation2> Solve(List<int> listToSolve)
        {
            var loops = new List<LoopEquation2>();
            Debug.WriteLine("########################");
            Debug.WriteLine("Solving for " +string.Join(",",listToSolve));
            Debug.WriteLine("########################");

            var solver = new SortSolver(listToSolve);

            var watch = new Stopwatch();
            watch.Start();

            solver.Solve();
            watch.Stop();
            Debug.WriteLine("Created search space in {0}ms", watch.ElapsedMilliseconds);

            FunTree.VisitToBottom(solver.Root, node => Debug.WriteLine("Visiting: " + node));

            Debug.WriteLine("");

            Debug.WriteLine("Leaf Count:  " + FunTree.LeafCount(solver.Root));

            Debug.WriteLine("");

            Debug.WriteLine("Found {0} results", solver.ResultNodes.Count);

            foreach (var node in solver.ResultNodes)
            {
                Debug.WriteLine("Result: " + node);

                var list = FunTree.VisitToTop(node).SelectMany(p=> p).ToList();
                watch.Restart();
                loops = ArithmeticLoopSolver.Solve(list);
                watch.Stop();
                Debug.WriteLine("Solved loops in {0}ms", watch.ElapsedMilliseconds);
                foreach (var loopEquation in loops)
                {
                    Debug.WriteLine("Found loop: " + loopEquation);
                    Debug.WriteLine(loopEquation.ToloopString("f({0});"));
                }
            }

            Debug.WriteLine("");

            return loops;
        }
    }
}
