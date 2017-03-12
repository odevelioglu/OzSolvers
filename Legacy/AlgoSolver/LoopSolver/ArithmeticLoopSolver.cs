using System.Collections.Generic;

namespace OzAlgo.LoopSolver
{
    public class ArithmeticLoopSolver
    {
        public static List<LoopEquation2> Solve(List<int> list)
        {
            var ret = new List<LoopEquation2>();

            var arithmetic1Solutions = new Arithmeric1LoopSolver(list).Solve();
            ret.AddRange(arithmetic1Solutions);

            var arithmetic2Solutions = new Arithmeric2LoopSolver(list).Solve();
            ret.AddRange(arithmetic2Solutions);

            return ret;
        }
    }
}
