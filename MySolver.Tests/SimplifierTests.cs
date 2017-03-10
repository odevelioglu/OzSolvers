using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySolver.Inferring;

namespace MySolver.Tests
{
    [TestClass]
    public class SimplifierTests
    {
        private BinaryFunction div = new BinaryFunction("div", vars => vars[0] / vars[1], 2);
        private BinaryFunction fac = new BinaryFunction("fac", vars => MathHelper.Fact((int)vars[0]), 1);
        private Node c2 = new Node(new Constant(2), null, null);
        private Node c1 = new Node(new Constant(1), null, null);
        
        [TestMethod]
        public void TestMethod13()
        {
            var fact1 = new Node(this.fac, c1, null);
            var tmp = new Node(this.div, c2, fact1);
            
            var simplified = Simplifier.Simplify(tmp);

            Assert.AreEqual("2", simplified.ToString());
        }

        [TestMethod]
        public void TestMethod2()
        {
            var inferer = new DifferenceTableInferer();
            var sequence = new[] { 1, 2, 3, 1, 2, 1 };
            var node = inferer.InternalInfer(sequence);
            
            Assert.AreEqual("(((((1 + (((n - 1) / (1)!) * 1)) + ((((n - 1) * (n - 2)) / (2)!) * 0)) + (((((n - 1) * (n - 2)) * (n - 3)) / (3)!) * -3)) + ((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) / (4)!) * 9)) + (((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) * (n - 5)) / (5)!) * -20))", node.ToString());

            var simplified = Simplifier.Simplify(node);

            Assert.AreEqual("(((n + (((((n - 1) * (n - 2)) * (n - 3)) / (3)!) * -3)) + ((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) / (4)!) * 9)) + (((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) * (n - 5)) / (5)!) * -20))", simplified.ToString());
        }

        [TestMethod]
        public void TestMethod3()
        {
            var inferer = new DifferenceTableInferer();

            //1+2+3+...+n= 
            var toSolve = new Func<int, double>(n => Enumerable.Range(1, n).Sum());
            var sequence = Enumerable.Range(1, 10).Select(n => (int)toSolve(n)).ToArray(); // 1, 3, 6, 10, 15, 21, 28
            var node = inferer.InternalInfer(sequence);

            Assert.AreEqual("(((((((((1 + (((n - 1) / (1)!) * 2)) + ((((n - 1) * (n - 2)) / (2)!) * 1)) + (((((n - 1) * (n - 2)) * (n - 3)) / (3)!) * 0)) + ((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) / (4)!) * 0)) + (((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) * (n - 5)) / (5)!) * 0)) + ((((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) * (n - 5)) * (n - 6)) / (6)!) * 0)) + (((((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) * (n - 5)) * (n - 6)) * (n - 7)) / (7)!) * 0)) + ((((((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) * (n - 5)) * (n - 6)) * (n - 7)) * (n - 8)) / (8)!) * 0)) + (((((((((((n - 1) * (n - 2)) * (n - 3)) * (n - 4)) * (n - 5)) * (n - 6)) * (n - 7)) * (n - 8)) * (n - 9)) / (9)!) * 0))", node.ToString());

            var simplified = Simplifier.Simplify(node);

            Assert.AreEqual("((1 + ((n - 1) * 2)) + (((n - 1) * (n - 2)) / (2)!))", simplified.ToString());
        }
    }
    
    // Sparse Representation : Read here https://www.usna.edu/Users/cs/roche/courses/cs487/mvpoly.pdf Read also "Symbolic Computation"
    // f = c1 x^e1 + c2 x^e2 +    + ct x^et
    // ((c1; e1); (c2; e2); : : : ; (ct; et))
    public class MultivariatePolinomial
    {
        public List<Tuple<double, double>> Coefcient_Exponent_Tuples;
    }
}
