using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzAlgo;
using System.Collections.Generic;
using System.Diagnostics;

namespace AlgoSolverTests
{
    [TestClass]
    public class FunNodeTests
    {
        [TestMethod]
        public void Test_Visit()
        {
            var root = new FunNode() {Args = new List<int>{0}};

            var child1 = new FunNode() {Args = new List<int>{1}};
            root.AddChild(child1);
            var child2 = new FunNode() {Args = new List<int>{2}};
            root.AddChild(child2);

            var visited = new List<List<int>>();
            FunTree.VisitToBottom(root, node => visited.Add(node.Args));

            Assert.IsTrue(visited.SequenceEqualList(new List<List<int>> { new List<int>{ 0 }, new List<int>{ 1 }, new List<int>{ 2 } }));

            Assert.AreEqual(child1.Depth, 2);

            FunTree.VisitToBottom(root, node => Debug.WriteLine(node));

            // reverse visit
            visited.Clear();
            FunTree.VisitToTop(child1, node => visited.Add(node.Args));

            Assert.IsTrue(visited.SequenceEqualList(new List<List<int>> { new List<int> { 1 }, new List<int> { 0 } }));
        }

        [TestMethod]
        public void Print_Visit()
        {
            var indexes = new[] { new List<int> { 0 }, new List<int> { 1 }, new List<int> { 2 } };
            var root = new FunNode();

            foreach (var x in indexes)
            {
                var child = new FunNode {Args = x};
                root.AddChild(child);
            }

            FunTree.VisitToBottom(root, node => Debug.WriteLine(node));
        }
    }
}
