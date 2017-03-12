using System.Collections.Generic;

namespace OzAlgo.LoopSolver
{
    interface IArithmericLoopSolver
    {
        List<LoopEquation2> Solve();
    }
    
    /// <summary>
    /// f(n) = k * n + d
    /// EX: 3,5,7,9,11 => f(n) = 2n+1 n<=5
    /// </summary>
    public class Arithmeric1LoopSolver : IArithmericLoopSolver
    {
        private readonly List<int> list;

        public Arithmeric1LoopSolver(List<int> list)
        {
            this.list = list;
        }

        public List<LoopEquation2> Solve()
        {
            var ret = new List<LoopEquation2>();

            if (HasUniqueDistance())
            {
                ret.Add(new LoopEquation1()
                {
                    Criterion = new LoopGroupEquation2() {En = list.Count},
                    k = list[1] - list[0],
                    d = 2*list[0] - list[1]
                });
            }

            return ret;
        }

        public bool HasUniqueDistance()
        {
            if (list.Count < 2)
                return false;

            var firstD = list[1] - list[0];
            for (int i = 1; i < list.Count - 1; i++)
            {
                if (list[i + 1] - list[i] != firstD)
                    return false;
            }

            return true;
        }
    }
}
