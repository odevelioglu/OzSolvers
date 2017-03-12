using System.Collections.Generic;

namespace OzAlgo.LoopSolver
{
    interface IGeometricLoopSolver
    {
        List<GeometricLoopEquation> Solve();
    }

    /// <summary>
    /// f(n) = k * n^r
    /// EX: 2, 12, 24, 48 => f(n) = 3*n^2 n<=4
    /// </summary>
    public class Geometric11LoopSolver : IGeometricLoopSolver
    {
        private readonly List<int> list;

        public Geometric11LoopSolver(List<int> list)
        {
            this.list = list;
        }

        public List<GeometricLoopEquation> Solve()
        {
            var ret = new List<GeometricLoopEquation>();

            if (HasUniqueDistance())
            {
                ret.Add(new GeometricLoopEquation()
                {
                    En = list.Count,
                    r = list[1] / list[0],
                    k = list[0] * list[0] / list[1]
                });
            }

            return ret;
        }

        public bool HasUniqueDistance()
        {
            if (list.Count < 2)
                return false;
            
            var rate = list[1] / list[0];
            for (int i = 1; i < list.Count - 1; i++)
            {
                if (list[i + 1] / list[i] != rate)
                    return false;
            }

            return true;
        }
    }

    public class GeometricLoopEquation
    {
        public int k;
        public int r;
        public int En;

        public override string ToString()
        {
            return string.Format("f(n) = {0}*{1}^n  n<{2}",
                k,
                r,
                En);
        }
    }
}
