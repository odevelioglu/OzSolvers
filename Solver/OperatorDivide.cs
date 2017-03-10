namespace Solver
{
    public class OperatorDivide : Operator
    {
        public override bool isOperator()
        {
            return true;
        }

        public override double eval(double[] vars, BinaryTree<ExpressionPart> subTree)
        {
            if (subTree.getLeft() != null && subTree.getRight() != null)
            {
                double RValue = subTree.getRight().getRoot().eval(vars, subTree.getRight()); // do right value first to check if divide by 0
                double LValue = subTree.getLeft().getRoot().eval(vars, subTree.getLeft());
                if (RValue != 0)
                    return LValue / RValue;
                else
                    return LValue / (RValue + 0.0000000001f); // should throw divide by 0 exception here
            }
            else
                return 0;
        }

        public override BinaryTree<ExpressionPart> applyTo(BinaryTree<ExpressionPart> lhs, double rhs)
        {
            return new BinaryTree<ExpressionPart>(this, lhs, new Constant(rhs));
        }

        public override string toString()
        {
            return "/";
        }

        public override ExpressionType getType()
        {
            return ExpressionType.OPERATOR_DIV;
        }

        public override bool isOperand()
        {
            return false;
        }

        public override bool isUnary() { return false; }

        
    }
}
