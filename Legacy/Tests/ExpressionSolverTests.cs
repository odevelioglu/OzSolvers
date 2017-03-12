using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzCompress;
using System;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class ExpressionSolverTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var x = Expression.Parameter(typeof(int));
            var y = Expression.Parameter(typeof(int));
            var sumCall = Expression.Call(typeof(ExpressionSolver), "Sum", null, x, y);

            var lambda = Expression.Lambda<Func<int, int, int>>(sumCall, x, y);

            var method = lambda.Compile();

            Assert.AreEqual(method(2,3), 5);
            Assert.AreEqual(ExpressionSolver.SumCount, 1);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var x = Expression.Parameter(typeof(int));
            var y = Expression.Parameter(typeof(int));
            var ret = Expression.Variable(typeof(int), "ret");

            var sumCall = Expression.Call(typeof(ExpressionSolver), "Sum", null, x, y);

            var returnTarget = Expression.Label(typeof(int));

            var block = Expression.Block(
                new [] { ret },
                Expression.Assign(ret, sumCall),
                Expression.Label(returnTarget, ret));

            var lambda = Expression.Lambda<Func<int, int, int>>(block, x, y);
            var compiled = lambda.Compile();
            
            Assert.AreEqual(compiled(2, 3), 5);
        }

        [TestMethod]
        public void TestMethod3()
        {
            var x = Expression.Parameter(typeof(int), "x");
            var y = Expression.Parameter(typeof(int), "y");
            var a = Expression.Variable(typeof(int), "a");
            var b = Expression.Variable(typeof(int), "b");

            var sumCall = Expression.Call(typeof(ExpressionSolver), "Sum", null, x, y);
            var mulCall = Expression.Call(typeof(ExpressionSolver), "Mul", null, a, y);

            var returnTarget = Expression.Label(typeof(int));

            BlockExpression block = Expression.Block(
                new [] { a, b },
                new Expression[]{ 
                    Expression.Assign(a, sumCall),
                    Expression.Assign(b, mulCall),
                    Expression.Label(returnTarget, b)
                }
            );

            

            var lambda = Expression.Lambda<Func<int, int, int>>(block, x, y);
            var compiled = lambda.Compile();

            Assert.AreEqual(compiled(2, 3), 15);
            Assert.AreEqual(ExpressionSolver.MulCount, 1);
            Assert.AreEqual(ExpressionSolver.SumCount, 1);
        }
    }
}
