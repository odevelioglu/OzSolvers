using System;
using MySolver.Inferring;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace MySolver.Tests.InferringTests
{
    [TestFixture]
    public class LLLInfererTests
    {
        [Test]
        public void RationalInfererAll()
        {
            var lll = new LLL_Lattice();

            var b = new double[,] { {1, -1, 3}, {1,0,5}, {1,2,6} };


            //lll.LLL(3, b, )
            //Assert.AreEqual(new Tuple<long, long>(1, 503), res);
        }
    }
}
