namespace OzAlgo.LoopSolver
{
    public class EquationString
    {
        private readonly string _variableName;
        private readonly int _coef;

        public EquationString(int coef, string variableName)
        {
            _coef = coef;
            _variableName = variableName;
        }

        public EquationString(int coef) : this(coef, "") { }
        public EquationString(string variableName) : this(1, variableName) { }

        public static string operator +(EquationString eq1, EquationString eq2)
        {
            return eq1.ToString() + eq2;
        }

        public static string operator +(string eq1, EquationString eq2)
        {
            if (string.IsNullOrEmpty(eq1) &&
                string.IsNullOrEmpty(eq2.ToString()))
                return "";

            if (string.IsNullOrEmpty(eq2.ToString()))
                return eq1;

            if (string.IsNullOrEmpty(eq1))
                return eq2.ToString();

            if (eq2.ToString().StartsWith("-"))
                return eq1 + eq2.ToString();

            return eq1 + "+" + eq2.ToString();
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(_variableName))
            {
                switch (_coef)
                {
                    case 0:
                        return "";
                    case 1:
                        return _variableName;        // n
                    case -1:
                        return "-" + _variableName;  // -n
                    default:
                        return _coef + _variableName;    // 2n
                }
            }

            //constant
            switch (_coef)
            {
                case 0:
                    return "";
                default:
                    return _coef.ToString();
            }
        }
    }
}
