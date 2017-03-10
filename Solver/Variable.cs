namespace Solver
{
    public class Variable : Operand
    {
        private int index;

        public Variable(int index)
        {
            this.index = index;
        }

        public override bool isOperator()
        {
            return false;
        }

        public override double eval(double[] vars, BinaryTree<ExpressionPart> subTree)
        {
            if (index >= 0 && index < vars.Length)
                return vars[index];
            else
                return 0; // should probably throw an error here
        }

        public string toString()
        {
            return "V" + index;
        }

        public override ExpressionType getType()
        {
            return ExpressionType.VARIABLE;
        }

        public override bool isOperand() => true;
        
    }
}
