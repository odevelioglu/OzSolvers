using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver
{
    class Program
    {
        static void Main(string[] args)
        {
            //var c2 = new Constant(2);
            //var c3 = new Constant(3);
            //var c4 = new Constant(4);
            //var add = new OperatorAdd();
            //var mul = new OperatorMultiply();
            //var div = new OperatorDivide();

            //var tree = new BinaryTree<ExpressionPart>(mul, c2, c3);

            //var res = tree.getRoot().eval(new double[](), expression);



            Expression exp;

            for (int i = 0; i < 5; i++)
            {
                exp = new Expression(1, (int)(new Random().NextDouble() * 10), -10, 10, 2);
                exp.setChances(1, 1, 1, 1, 1, 1, 1, 1);
                exp.addAllVars();
                exp.expand(6);
                exp.simplify();
                Console.WriteLine(("" + exp).Replace("V0", "x"));

            }
        }
    }


    public enum ExpressionType
    {
        VARIABLE,

        CONSTANT,

        OPERATOR_ADD,

        OPERATOR_SUB,

        OPERATOR_MULT,

        OPERATOR_DIV,

        OPERATOR_POW,

        OPERATOR_SIN,

        OPERATOR_COS,

        OPERATOR_TAN
    }

    public abstract class ExpressionPart
    {
        public abstract ExpressionType getType();

        public abstract bool isOperand();

        public abstract bool isOperator();

        public abstract double eval(double[] vars, BinaryTree<ExpressionPart> subTree);
    }


    public abstract class Operand : ExpressionPart
    {

    }

    public abstract class Operator : ExpressionPart
    {
        public abstract BinaryTree<ExpressionPart> applyTo(BinaryTree<ExpressionPart> lhs, double rhs);

        public abstract string toString();

        public abstract bool isUnary();
    }

}
