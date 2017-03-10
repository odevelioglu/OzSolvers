using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver
{
    public class Constant : Operand
    {
        private double value;

        public Constant(double value)
        {
            this.value = value;
        }

        public override double eval(double[] vars, BinaryTree<ExpressionPart> subTree)
        {
            return value;
        }

        public double getValue()
        {
            return value;
        }

        public string toString()
        {
            var result = "" + value;
            if (result.EndsWith(".0"))
                result = result.Replace(".0", "");

            return result;
        }

        public override ExpressionType getType()
        {
            return ExpressionType.CONSTANT;
        }
    }
}
