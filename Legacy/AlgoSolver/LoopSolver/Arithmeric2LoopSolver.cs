using System.Diagnostics;
using Microsoft.SolverFoundation.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OzAlgo.LoopSolver
{
    public class Arithmeric2LoopSolver : IArithmericLoopSolver
    {
        //public static Dictionary<int, List<LoopGroupEquation2>> _cache = new Dictionary<int, List<LoopGroupEquation2>>();
        private readonly List<int> list;

        public Arithmeric2LoopSolver(List<int> list)
        {
            this.list = list;
        }

        public List<LoopEquation2> Solve()
        {
            var results = new LoopGroupSolver2().Solve(list.Count);

            var ret = new List<LoopEquation2>();

            foreach (var criterion in results)
            {
                //Debug.WriteLine("Solving for En={0} k={1} d={2}",
                //    criterion.En, criterion.kpp, criterion.dpp);

                var sol = SolveLoop(list, criterion.En, criterion.kpp, criterion.dpp);
                if (sol.GetReport().SolutionQuality == SolverQuality.Optimal)
                {
                    ret.Add(new LoopEquation2()
                    {
                        Criterion = criterion,
                        k = sol.GetDecisionValue("k"),
                        d = sol.GetDecisionValue("d"),
                        kp = sol.GetDecisionValue("kp"),
                        dp = sol.GetDecisionValue("dp"),
                    });

                    //foreach (var decision in sol.Decisions)
                    //{
                    //    Debug.WriteLine(decision.ToString());
                    //}
                }
            }

            return ret;
        }

        // TODO: Replace this by "simultaneos linear equation solving"
        public static Solution SolveLoop(List<int> list, int En, int kpp, int dpp)
        {
            //Debug.WriteLine("Solving for list:{0}, En:{1}, kpp:{2}, dpp{3}", string.Join(",", list), En, kpp, dpp);

            var context = SolverContext.GetContext();
            context.ClearModel();
            var model = context.CreateModel();

            var k = new Decision(Domain.Integer, "k");
            var d = new Decision(Domain.Integer, "d");
            var kp = new Decision(Domain.Integer, "kp");
            var dp = new Decision(Domain.Integer, "dp");

            model.AddDecisions(k, d, kp, dp);

            var ptr = 0;
            for (var n = 1; n <= En; n++)
            {
                for (var m = 1; m <= kpp*n + dpp; m++)
                {
                    model.AddConstraint("eq" + (ptr + 10),
                        list[ptr] == k*n*m + d*m + kp*n + dp);

                    ptr++;
                }
            }

            var solution = context.Solve();

            return solution;
        }
    }

    public static class SolutionExtensions
    {
        public static int GetDecisionValue(this Solution solution, string name)
        {
            return Convert.ToInt32(solution.Decisions
                .FirstOrDefault(des => des.Name == name)
                .GetValues().First().First());
        }
    }
}
