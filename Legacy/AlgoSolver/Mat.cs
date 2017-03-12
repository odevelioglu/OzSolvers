using System;
using System.Collections.Generic;

namespace OzAlgo
{
    public class Mat
    {
        public static Dictionary<int, int> Cache_nLgn_Up = new Dictionary<int, int>();
        public static int nLgn_Up(int n)
        {
            int ret;
            if (Cache_nLgn_Up.TryGetValue(n, out ret))
                return ret;

            ret = (int) Math.Ceiling(n*Math.Log(n, 2));

            Cache_nLgn_Up.Add(n, ret);

            return ret;
        }

        public static Dictionary<int, int> Cache_LgFact_Up = new Dictionary<int, int>();
        public static int LgFact_Up(int n)
        {
            int ret;
            if (Cache_nLgn_Up.TryGetValue(n, out ret))
                return ret;

            ret = (int)Math.Ceiling(Math.Log(Fact(n), 2));

            Cache_nLgn_Up.Add(n, ret);

            return ret;
        }

        public static Dictionary<int, double> Cache_fact = new Dictionary<int, double>();
        public static double Fact(int n)
        {
            double ret;
            if (Cache_fact.TryGetValue(n, out ret))
                return ret;

            ret = 1;
            for (var i = 2; i <= n; i++)
            {
                ret *= i;
            }
            Cache_fact.Add(n, ret);
            return ret;
        }

        public static Dictionary<double, double> Cache_CountIf = new Dictionary<double, double>();
        public static double CountIf(double n)
        {
            double ret;
            if (Cache_CountIf.TryGetValue(n, out ret))
                return ret;

            if (n <= 1) return 0;

            if ((n % 2).Equals(0))
                return 2 * CountIf(n / 2) + n;

            ret = CountIf((n + 1) / 2) + CountIf((n - 1) / 2) + n;

            Cache_CountIf.Add(n, ret);
            return ret;
        }

        
    }
}
