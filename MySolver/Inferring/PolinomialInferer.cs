using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System.Globalization;
using System.Linq;

namespace MySolver.Inferring
{
    public class PolinomialInferer
    {
        public Poli Infer(int[] list)
        {
            var dobList = list.Select(Convert.ToDouble).ToArray();

            for (int i = 1; i <= dobList.Length; i++)
            {
                var poli = InternalInfer(dobList.Sub(0, i));
                if (Verify(poli, list)) return poli;
            }

            return null;
        }

        private bool Verify(Poli poli, int[] list)
        {
            for (var i = 0; i < list.Length; i++)
            {
                if (!poli.Eval(i).Equals(list[i]))
                    return false;
            }

            return true;
        }

        public Poli InternalInfer(double[] b)
        {
            var A = this.CreateMatrix(b.Length);

            var sol = MatrixSolve(A, b);
            //var res = sol.ToString();

            return new Poli(sol);
        }

        public static double[] MatrixSolve(Matrix<double> A, double[] b)
        {
            var B = Vector<double>.Build;
            var vec = B.DenseOfArray(b);

            var sol = A.Inverse().Multiply(vec);
            //var res = sol.ToString();

            return sol.AsArray();
        }
        
        private Matrix<double> CreateMatrix(int n)
        {
            var M = Matrix<double>.Build;

            var mat = new double[n,n];
            for (int c = 0; c < n; c++)
            {
                for (int r = 0; r < n; r++)
                {
                    mat[c, r] = c == 0 && r == 0 ? 1 : Math.Pow(c, r);
                }
            }

            var matrix = M.DenseOfArray(mat); 

            //var tmp = matrix.ToString();

            return matrix;
        }
    }

    public class Poli
    {
        public double[] Coefs { get; set; }
        public Poli(double[] coefs)
        {
            this.Coefs = coefs;
        }

        public double Eval(int n)
        {
            var res = 0.0;
            for (var i = 0; i < Coefs.Length; i++)
            {
                res += Coefs[i] * Math.Pow(n, i);
            }
            return res;
        }

        public override string ToString()
        {
            var list = new List<string>();
            for(var power = 0; power < this.Coefs.Length; power++)
            {
                var coef = this.Coefs[power];
                if (coef.Equals(0)) continue;

                var coefStr = coef.ToString(CultureInfo.InvariantCulture);
                if (coef.Equals(1) && power != 0)
                    coefStr = string.Empty;

                var powerStr = string.Empty; // power == 0

                if (power == 1)
                {
                    powerStr = "n";
                }
                else if(power > 1)
                {
                    powerStr = $"n^{power}";
                }

                list.Add($"{coefStr}{powerStr}");
            }
            
            return string.Join(" + ", list);
        }
    }
}
