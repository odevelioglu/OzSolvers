using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzAlgo;
using System.Collections.Generic;
using System.Diagnostics;

namespace AlgoSolverTests
{
    [TestClass]
    public class SortSolver3Tests
    {
        [TestMethod]
        //[Ignore]
        public void Print_Solve4()
        {
            var watch = new Stopwatch();
            watch.Start();

            //Solve(new List<int>() { 9 });
            //Solve(new List<int>() { 9, 5 });
            Solve(new List<int>() { 9, 5, 4 });
            //Solve(new List<int>() { 9, 5, 4, 2 });

            watch.Stop();
            Debug.WriteLine("Overall time: {0}ms", watch.ElapsedMilliseconds);
        }

        private static void Solve(List<int> listToSolve)
        {
            Debug.WriteLine("########################");
            Debug.WriteLine("Solving for " + string.Join(",", listToSolve));
            Debug.WriteLine("########################");

            var solver = new SortSolver3(listToSolve);

            var watch = new Stopwatch();
            watch.Start();

            solver.Solve();
            watch.Stop();
            Debug.WriteLine("Created search space in {0}ms", watch.ElapsedMilliseconds);

            Debug.WriteLine("Leaf Count:  " + LeafCount(solver.Root));

            Debug.WriteLine("");

            //var str = new StringBuilder();

            var nodes = new List<FunNode>();
            Visit(solver.Root, nodes.Add);

            foreach (var node in nodes)
            {
                PrintToTop(node);
                Debug.WriteLine("");
            }
            
            Debug.WriteLine("");
        }

        public static void PrintToTop(FunNode node)
        {
            if (node == null || node.IsRoot)
                return;

            if (node.Function is FuncBaseIfLt)
            {
                Debug.Write(node.Function.FuncName + "(" + string.Join(",", node.Args) + ")");

                Debug.Write("{ ");
                PrintToTop(node.ParentNode);
                Debug.Write(" }; ");
            }
            else
            {
                Debug.Write(node.Function.FuncName + "(" + string.Join(",", node.Args) + "); ");
                
                PrintToTop(node.ParentNode);
            }
        }


        public static void Visit(FunNode node, Action<FunNode> action)
        {
            if (node == null)
                return;

            action(node);

            foreach (var child in node.Childs)
            {
                Visit(child, action);
            }

            if (node.SubCalls.Any())
            {
                foreach (var child in node.SubCalls)
                {
                    Visit(child, action);
                }
            }
        }

        public static int LeafCount(FunNode root)
        {
            int i = 0;

            Visit(root, node =>
            {
                if (!node.HasChild)
                    i++;
            });

            return i;
        }

    }
}
