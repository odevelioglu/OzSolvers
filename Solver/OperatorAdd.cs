using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver
{
    public class OperatorAdd : Operator
    {
        public override bool isOperator()
        {
            return true;
        }

        public override double eval(double[] vars, BinaryTree<ExpressionPart> subTree)
        {
            if (subTree.getLeft() != null && subTree.getRight() != null)
            {
                double LValue = subTree.getLeft().getRoot().eval(vars, subTree.getLeft());
                double RValue = subTree.getRight().getRoot().eval(vars, subTree.getRight());
                return LValue + RValue;
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
            return "+";
        }

        public override ExpressionType getType()
        {
            return ExpressionType.OPERATOR_ADD;
        }

        public override bool isOperand()
        {
            return false;
        }

        public override bool isUnary() { return false; }


    }
}
