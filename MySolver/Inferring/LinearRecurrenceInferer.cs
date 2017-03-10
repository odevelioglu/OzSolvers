using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Linq;

namespace MySolver.Inferring
{
    using System.Globalization;
    using System.Runtime.Remoting.Metadata.W3cXsd2001;

    using MathNet.Numerics;

    public class LinearRecurrenceInferer
    {
        public Recurrence Infer(int[] list)
        {
            return Infer(list.Select(Convert.ToDouble).ToArray());
        }

        public Recurrence Infer(double[] list)
        {
            if (list.Length % 2 == 1)
                list = list.First(list.Length - 1);

            var n = list.Length;

            var MforRank = this.CreateM(list.First(n - 2));
            var tm = MforRank.ToString();
            
            var k = MforRank.Rank();

            var M = CreateM(list.First(2 * k));
            var ttt = M.ToString();

            var list2 = list.Sub(k,  k);
            var sol = PolinomialInferer.MatrixSolve(M, list2);

            return new Recurrence { BaseCases = list.First(k), Coefs = sol, Degree = k};
        }

        private Matrix<double> CreateM(double[] list)
        {
            var M = Matrix<double>.Build;

            var j = list.Length / 2;//floor
              
            var mat = new double[j, j];
            for (var c = 0; c < j; c++)
            {
                for (var r = 0; r < j; r++)
                {
                    mat[c, r] = list[r + c];
                }
            }

            var matrix = M.DenseOfArray(mat);

            var tmp = matrix.ToString();

            return matrix;
        }
    }

    public class Recurrence
    {
        public double[] BaseCases;

        public double[] Coefs;

        public int Degree;

        public override string ToString()
        {
            var ret = new StringBuilder();
            for(var i = 0; i < this.Degree; i++)
            {
                if (ret.Length > 0) ret.Append(" ");
                ret.Append($"f({i}) = {this.BaseCases[i]}");
            }

            if (ret.Length > 0) ret.Append(" ");

            var str = new List<string>();
            for (var i = 0; i < this.Degree; i++)
            {
                var decim = this.Coefs[this.Degree - i - 1];
                
                str.Add($"{ToPettyString(decim)} f(n - {i + 1})");
                
            }
            ret.Append($"f(n) = {string.Join(" + ", str)}" );
            return ret.ToString();
        }

        public string ToPettyString(double decim)
        {
            if (double.IsNaN(decim)) return "NaN";

            if (decim != (int)decim)
            {
                var div = ToDivision((decimal)decim);
                if (div != null)
                    return $"({div.Item1} / {div.Item2})";
            }

            return decim.ToString(CultureInfo.InvariantCulture);
        }

        public Tuple<long, long> ToDivision(decimal decim)
        {
            var epsilon = (decimal)Math.Pow(10, -10);
            var areEqual = new Func<decimal, decimal, bool>((r, p) => Math.Abs(r - p) < epsilon); // use Precision.Almostequal
            
            var convergent = new ConvergentCalculator();
            
            foreach (var d in ContinuedFraction.FractionIterator(decim))
            {
                convergent.Add(d);

                if (areEqual(decim, (decimal)convergent.R / convergent.P))
                    return new Tuple<long, long>(convergent.R, convergent.P);
            }

            return null;
        }
    }
}
