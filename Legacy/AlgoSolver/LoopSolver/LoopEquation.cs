using System.Text;

namespace OzAlgo.LoopSolver
{
    public class LoopEquation2
    {
        public int k;
        public int d;
        public int kp;
        public int dp;
        public LoopGroupEquation2 Criterion;

        public override string ToString()
        {
            return string.Format("f(n,m) = {0} [{1}]",
               new EquationString(k, "nm") + new EquationString(d, "m") + new EquationString(kp, "n") + new EquationString(dp),
               Criterion);
        }

        public string ToloopString(string func)
        {
            var str = new StringBuilder();
            str.AppendFormat("for(n: 1..{0})", Criterion.En);
            str.AppendLine();
            str.AppendFormat("   for(m: 1..{0})", new EquationString(Criterion.kpp, "n") + new EquationString(Criterion.dpp) );
            str.AppendLine();
            str.AppendFormat("      " + func,
                new EquationString(k, "nm") + new EquationString(d, "m") + new EquationString(kp, "n") + new EquationString(dp));
            str.AppendLine();

            return str.ToString();
        }
    }
    
    public class LoopEquation1 : LoopEquation2
    {
        public int Compute(int x)
        {
            return x * k + d;
        }

        public override string ToString()
        {
            return string.Format("f(n) = {0}  n<{1}",
                new EquationString(k, "n") + new EquationString(d),
                Criterion.En);
        }
    }
}
