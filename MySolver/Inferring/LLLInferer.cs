using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySolver.Inferring
{
    public class LLLInferer
    {

    }

    public class LLL_Lattice
    {
        private double Scalar(long n, double[] u, double[] v)
        {
            // Calculate the scalar product of two vectors [1..n] 
            double sum = 0.0;

            for (int i = 1; i <= n; i++) sum += u[i] * v[i];
            return sum;
        }

        private void RED(int k, int l, int n,
                         double[,] b, double[,] h,
                         double[,] mu)
        {
            int i, q = (int)(0.5 + mu[k, l]);

            if (Math.Abs(mu[k, l]) > 0.5)
            {
                for (i = 1; i <= n; i++)
                {
                    b[k, i] -= q * b[l, i];
                    h[i, k] -= q * h[i, l];
                }
                mu[k, l] -= q;
                for (i = 1; i <= l - 1; i++) mu[k, i] -= q * mu[l, i];
            }
        }

        private void SWAP(int k, int k1, int kmax, int n,
                          double m, double[] B, double[,] b,
                          double[,] bs, double[,] h, double[,] mu)
        {
            double C, t;
            double[] c = new double[n + 1];
            int i, j;

            for (j = 1; j <= n; j++)
            {
                t = b[k, j];
                b[k, j] = b[k1, j];
                b[k1, j] = t;
                t = h[j, k];
                h[j, k] = h[j, k1];
                h[j, k1] = t;
            }
            if (k > 2)
            {
                for (j = 1; j <= k - 2; j++)
                {
                    t = mu[k, j];
                    mu[k, j] = mu[k1, j];
                    mu[k1, j] = t;
                }
            }
            C = B[k] + m * m * B[k1];
            mu[k, k1] = m * B[k1] / C;
            for (i = 1; i <= n; i++) c[i] = bs[k1, i];
            for (i = 1; i <= n; i++)
                bs[k1, i] = bs[k, i] + m * c[i];
            for (i = 1; i <= n; i++)
                bs[k, i] = -m * bs[k, i] + B[k] * c[i] / C;
            B[k] *= B[k1] / C;
            B[k1] = C;
            for (i = k + 1; i <= kmax; i++)
            {
                t = mu[i, k];
                mu[i, k] = mu[i, k1] - m * t;
                mu[i, k1] = t + mu[k, k1] * mu[i, k];
            }
        }

        public bool LLL(int n, double[,] b, double[,] h)
        {
            // Lattice reduction algorithm 
            double m;
            double[] B = new double[n + 1];
            double[] bv = new double[n + 1];
            double[] bw = new double[n + 1];
            double[,] bs = new double[n + 1, n + 1];
            double[,] mu = new double[n + 1, n + 1];
            int i, j, k, k1, kmax = 1, l;

            for (i = 1; i <= n; i++)
                bv[i] = bs[1, i] = b[1, i];

            B[1] = Scalar(n, bv, bv);

            for (i = 1; i <= n; i++)
            {
                for (j = 1; j <= n; j++)
                    h[i, j] = 0.0;
                h[i, i] = 1.0;
            }
            for (k = 2; k <= n; k++)
            {
                int cs = 2;

                switch (cs)
                {
                    case 2:
                        if (k <= kmax)
                            goto case 3;
                        kmax = k;
                        for (i = 1; i <= n; i++) bs[k, i] = b[k, i];
                        for (j = 1; j <= k - 1; j++)
                        {
                            for (l = 1; l <= n; l++)
                            {
                                bv[l] = b[k, l];
                                bw[l] = bs[j, l];
                            }
                            mu[k, j] = Scalar(n, bv, bw) / B[j];
                            for (i = 1; i <= n; i++)
                                bs[k, i] -= mu[k, j] * bs[j, i];
                        }
                        for (i = 1; i <= n; i++)
                            bv[i] = bs[k, i];

                        B[k] = Scalar(n, bv, bv);
                        if (B[k] == 0.0)
                            return false;
                        goto case 3;

                    case 3:
                        k1 = k - 1;
                        m = mu[k, k1];
                        RED(k, k1, n, b, h, mu);
                        if (B[k] < (0.75 - m * m) * B[k1])
                        {
                            SWAP(k, k1, kmax, n, m, B, b, bs, h, mu);
                            k = Math.Max(2, k1);
                            goto case 3;
                        }
                        for (l = k - 2; l >= 1; l--)
                            RED(k, l, n, b, h, mu);
                        break;
                }
            }

            return true;
        }
    }

}
