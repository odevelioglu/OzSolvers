using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OzCompress
{
 
    public class ExpressionSolver
    {

        List<int> vars = new List<int>(){2,3,5};

        Dictionary<int, int> varUsage = new Dictionary<int, int>();

        public static int SumCount = 0;
        public static int MulCount = 0;
        public static int Sum(int x, int y)
        {
            SumCount++;
            return x + y;
        }

        static int Mul(int x, int y)
        {
            MulCount++;
            return x * y;
        }
        
        bool Test(int x)
        {
            return x == 18;
        }

        private List<Func<int, int, int>> funcs = new List<Func<int, int, int>>();
        public void Solve()
        {
            var sum = new Func<int, int, int>(Sum);
            var mul = new Func<int, int, int>(Mul);

            funcs.Add(sum);
            funcs.Add(mul);

            var x = Expression.Parameter(typeof(int));
            var y = Expression.Parameter(typeof(int));
            var sumCall = Expression.Call(typeof(ExpressionSolver), "Sum", null, x, y);



            varUsage.Add(2, 0);
            varUsage.Add(3, 0);
            varUsage.Add(5, 0);

            GenerateExp(Expression.Empty());
        }

        void GenerateExp(Expression exp)
        {

            

            //foreach (var func in funcs)
            //{
            //    foreach (var i in vars)
            //    {
            //        //func(
            //    }
            //}
        }

    }
}
