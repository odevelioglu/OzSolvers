using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzAlgo.LoopSolver;

namespace AlgoSolverTests
{
    [TestClass]
    public class EquationStringTests
    {
        [TestMethod]
        public void Test()
        {
            var eq1 = new EquationString(2, "n");
            var eq2 = new EquationString(3, "m");

            Assert.AreEqual(eq1+eq2, "2n+3m");

            Assert.AreEqual(new EquationString(0, "n") + new EquationString(0, "m"), "");
            Assert.AreEqual(new EquationString(2, "n") + new EquationString(0, ""), "2n");
            Assert.AreEqual(new EquationString(0, "n") + new EquationString(3, "m"), "3m");
            
            Assert.AreEqual(new EquationString(0, "n") + new EquationString(5), "5");
            Assert.AreEqual(new EquationString(1, "m") + new EquationString(0, "n") + new EquationString(5), "m+5");

            var str = new EquationString(2, "nm") + new EquationString(1, "m") + new EquationString(0, "n") +
                      new EquationString(5);
            Assert.AreEqual(str, "2nm+m+5");

        }

    }
}
