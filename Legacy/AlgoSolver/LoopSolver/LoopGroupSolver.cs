using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SolverFoundation.Services;

namespace OzAlgo.LoopSolver
{
    public class LoopGroupSolver2
    {
        public static Dictionary<int, List<LoopGroupEquation2>> _cache = new Dictionary<int, List<LoopGroupEquation2>>();

        public LoopGroupSolver2() { }

        /// <summary>
        /// 1,2,3,1,2,1 => (1,2,3),(1,2),(1)
        ///             => (1,2),(3,1),(2,1)
        ///             => (1,2,3),(1,2,1)
        ///             => etc...
        /// </summary>
        public List<LoopGroupEquation2> Solve(int C)
        {
            if (_cache.ContainsKey(C))
                return _cache[C];

            //TODO: persist results to file

            var ret = new List<LoopGroupEquation2>();

            var context = SolverContext.GetContext();
            context.ClearModel();
            var model = context.CreateModel();

            var En = new Decision(Domain.Integer, "En");
            var kpp = new Decision(Domain.Integer, "kpp");
            var dpp = new Decision(Domain.Integer, "dpp");

            model.AddDecisions(En, kpp, dpp);
            model.AddConstraint("eq1", C == En * (En + 1) * kpp / 2 + (En * dpp));
            model.AddConstraint("eq2", En * kpp + dpp >= 1);
            model.AddConstraint("eq3", kpp + dpp >= 1);
            model.AddConstraint("eq4", C > En);
            model.AddConstraint("eq5", En > 1);

            var solution = context.Solve();

            while (solution.GetReport().SolutionQuality == SolverQuality.Feasible)
            {
                ret.Add(new LoopGroupEquation2()
                {
                    En = solution.GetDecisionValue("En"),
                    kpp = solution.GetDecisionValue("kpp"),
                    dpp = solution.GetDecisionValue("dpp")
                });

                solution.GetNext();
            }

            _cache.Add(C, ret);

            return ret;
        }
    }

    public class LoopGroupEquation2
    {
        public int En;
        public int kpp;
        public int dpp;

        public int C
        {
            get
            {
                return En * (En + 1) / 2 * kpp + En * dpp;
            }
        }

        public override string ToString()
        {
            return string.Format("n <= {0}, m <= {1}",
                En,
                new EquationString(kpp, "n") + new EquationString(dpp)
            );

        }

        public string GetValuesStr()
        {
            var str = new StringBuilder();

            foreach (var n in Enumerable.Range(1, En))
            {
                str.Append("(");
                str.Append(string.Join(", ", Enumerable.Range(1, n * kpp + dpp)));
                str.Append("), ");
            }

            return str.ToString();
        }
    }
}
