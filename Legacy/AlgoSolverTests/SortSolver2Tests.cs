using System;
using System.Linq;
using Facet.Combinatorics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzAlgo;
using System.Collections.Generic;
using System.Diagnostics;

namespace AlgoSolverTests
{
    [TestClass]
    public class SortSolver2Tests
    {
        [TestMethod]
        public void Print_CountIf()
        {
            var list = Enumerable.Range(1, 30).Select(Mat.Fact);

            Debug.WriteLine("n:\t\t\tf(n)\t\t\tlga!\t\t\t");
            foreach (var fact in list)
            {
                var fOfn = Mat.CountIf(fact)/fact;
                var lgn = Math.Log(fact, 2);
                var diffP = (fOfn - lgn) / lgn * 1000;
                Debug.WriteLine(fact + "\t\t\t" + fOfn + "\t\t\t" + lgn + "\t\t\t" + diffP);
            }
        }

        [TestMethod]
        [Ignore]
        public void Test_Combinations()
        {
            var list = Enumerable.Range(0, 3).ToList();

            var combinations = new Combinations<int>(list, 2, GenerateOption.WithoutRepetition);

            foreach (var combination in combinations)
            {
                Debug.WriteLine(string.Join(",", combination));
            }
        }

        [TestMethod]
        //[Ignore]
        public void Print_Solve4()
        {
            var watch = new Stopwatch();
            watch.Start();

            Solve(new List<int>() { 9 });
            Solve(new List<int>() { 9, 5 });
            Solve(new List<int>() { 9, 5, 4 });
            Solve(new List<int>() { 9, 5, 4, 2 });

            watch.Stop();
            Debug.WriteLine("Overall time: {0}ms", watch.ElapsedMilliseconds);
        }

        private static void Solve(List<int> listToSolve)
        {
            Debug.WriteLine("########################");
            Debug.WriteLine("Solving for " + string.Join(",", listToSolve));
            Debug.WriteLine("########################");

            var solver = new SortSolver2(listToSolve );

            var watch = new Stopwatch();
            watch.Start();

            solver.Solve();
            watch.Stop();
            Debug.WriteLine("Created search space in {0}ms", watch.ElapsedMilliseconds);

            Debug.WriteLine("Leaf Count:  " + FunTree.LeafCount(solver.Root));

            Debug.WriteLine("");

            FunTree.VisitToBottom(solver.Root, node => Debug.WriteLine("Visiting: " + node));

            Debug.WriteLine("");

            foreach (var node in GetFastestResults(solver.ResultNodes))
            {
                Debug.WriteLine("Result: " + node);
            }

            Debug.WriteLine("");
        }

        [TestMethod]
        //[Ignore]
        public void Test_PermutedInputs()
        {
            var list = new List<int>() { 1, 2, 3, 4 };
            var permuts = new Permutations<int>(list);

            foreach (List<int> permutation in permuts)
            {
                Debug.WriteLine("Solving for:" + string.Join(",", permutation));
                var solver = new SortSolver2(permutation);
                solver.Solve();

                foreach (var node in GetFastestResults(solver.ResultNodes))
                {
                    Debug.WriteLine("   " + node);
                }

                Debug.WriteLine("");
            }
        }

        [TestMethod]
        //[Ignore]
        public void Test_PermutedInputs_GetFastest()
        {
            var watch = new Stopwatch();
            watch.Start();
            var list = new List<int>() { 1, 2, 3, 4};
            var permuts = new Permutations<int>(list);

            var temp = new List<Tuple<string, string>>();

            foreach (List<int> permutation in permuts)
            {
                var solver = new SortSolver2(permutation);
                solver.Solve();

                var fastestResult = GetFastestResults(solver.ResultNodes).FirstOrDefault();

                if (fastestResult != null)
                    temp.Add(new Tuple<string, string>(string.Join(",", permutation), fastestResult.ToString()));
            }

            foreach (var item in temp.OrderBy(p => p.Item2.Length))
            {
                Debug.WriteLine(item.Item1 + "  " + item.Item2);
            }

            watch.Stop();
            Debug.WriteLine("Solved in {0}ms", watch.ElapsedMilliseconds);
        }

        public static IEnumerable<FunNode> GetFastestResults(List<FunNode> nodes)
        {
            int minDepth = 0;
            if (nodes.Count > 0)
                minDepth = nodes.Min(p => p.Depth);

            return nodes.Where(node => node.Depth == minDepth);
        }
    }
}
