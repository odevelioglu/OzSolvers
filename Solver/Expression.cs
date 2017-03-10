using System;

namespace Solver
{
    using System.Collections.Generic;

    public class Expression
    {
        private double CHANCE_ADD = 1.0, CHANCE_SUB = 1.0, CHANCE_MULT = 1.0, CHANCE_DIV = 1.0, CHANCE_POW = 1.0, CHANCE_SIN = 1.0, CHANCE_COS = 1.0, CHANCE_TAN = 1.0;

        private BinaryTree<ExpressionPart> expression;
        private int numVars;
        private double MIN_VAL, MAX_VAL;
        public static int decimalPlaces;
        
        public Expression(int numVars, double minVal, double maxVal)
        {
            expression = new BinaryTree<ExpressionPart>(new Constant(new Random().Next() * (maxVal - minVal) + minVal)));
            this.numVars = numVars;
            MIN_VAL = minVal;
            MAX_VAL = maxVal;
            decimalPlaces = -1;
        }

        public Expression(int numVars, double initConst, double minVal, double maxVal)
        {
            expression = new BinaryTree<ExpressionPart>(new Constant(initConst));
            this.numVars = numVars;
            MIN_VAL = minVal;
            MAX_VAL = maxVal;
            decimalPlaces = -1;
        }

        public Expression(int numVars, double initConst, double minVal, double maxVal, int roundTo)
        {
            expression = new BinaryTree<ExpressionPart>(new Constant(initConst));
            this.numVars = numVars;
            MIN_VAL = minVal;
            MAX_VAL = maxVal;
            decimalPlaces = roundTo;
        }

        public Expression(BinaryTree<ExpressionPart> initExpression, int numVars, double minVal, double maxVal)
        {
            expression = initExpression;
            this.numVars = numVars;
            MIN_VAL = minVal;
            MAX_VAL = maxVal;
            decimalPlaces = -1;
        }

        public Expression(BinaryTree<ExpressionPart> initExpression, int numVars, double minVal, double maxVal, int roundTo)
        {
            expression = initExpression;
            this.numVars = numVars;
            MIN_VAL = minVal;
            MAX_VAL = maxVal;
            decimalPlaces = -1;
        }

        // Copy constructor
        public Expression(Expression toCopy): this
        (
            new BinaryTree<ExpressionPart>(toCopy.expression),
            toCopy.numVars,
            toCopy.MIN_VAL,
            toCopy.MAX_VAL,
            decimalPlaces
        ){ }

        //------------------------------------
        //	[GENETIC] Cross two expressions
        //------------------------------------
        public static Expression cross(Expression e1, Expression e2)
        {
            return cross(e1, e2, 0.5f);
        }

        public static Expression cross(Expression e1, Expression e2, double bias)
        {
            BinaryTree<ExpressionPart> newExpTree_L = new BinaryTree<ExpressionPart>
            (
                new OperatorMultiply(),
                e1.expression,
                new Constant(1.0f - bias)
            );

            BinaryTree<ExpressionPart> newExpTree_R = new BinaryTree<ExpressionPart>
            (
                new OperatorMultiply(),
                e2.expression,
                new Constant(bias)
            );

            BinaryTree<ExpressionPart> newExpressionTree = new BinaryTree<ExpressionPart>
            (
                new OperatorAdd(),
                newExpTree_L,
                newExpTree_R
            );

            return new Expression
            (
                newExpressionTree,
                Math.Max(e1.numVars, e2.numVars),
                Math.Min(e1.MIN_VAL, e2.MIN_VAL), Math.Max(e1.MAX_VAL, e2.MAX_VAL)
            );
        }

        //------------------------------------
        //	[GENETIC] Mutate an expression (change it slightly)
        //------------------------------------
        public static Expression mutate(Expression e)
        {
            return mutate(e, 10);
        }

        public static Expression mutate(Expression exp, double constChange)
        {
            // Use copy constructor to make a result == exp
            Expression result = new Expression(exp);

            // Shift constants by a random amount
            for (int i = 0; i < result.expression.size(); i++)
            {
                if (result.expression.getValueAt(i).getType() == ExpressionType.CONSTANT)
                {
                    double randChange = round2((Math.Abs(constChange) * (2 * new Random().Next() - 1)), decimalPlaces);
                    double newVal = ((Constant)result.expression.getValueAt(i)).getValue() + randChange;
                    result.expression.setValueAt(i, new Constant(newVal));
                }
            }

            // Expand the expression between 0 - 1 times
            result.expand((int)(new Random().Next() * 2));

            return result;
        }

        //------------------------------------
        //	Evaluate the expression for given values of the variables
        //------------------------------------

        public double eval(double[] vars )
        {
            return expression.getRoot().eval(vars, expression);
        }

        //public void expand()
        //{
        //    int randTreeIndex = (int)(new Random().Next() * expression.size());

        //    List<Operator> potOps = new List<Operator>();
        //    if (new Random().Next() < CHANCE_ADD) potOps.Add(new OperatorAdd());
        //    if (new Random().Next() < CHANCE_SUB) potOps.Add(new OperatorSubtract());
        //    if (new Random().Next() < CHANCE_MULT) potOps.Add(new OperatorMultiply());
        //    if (new Random().Next() < CHANCE_DIV) potOps.Add(new OperatorDivide());
        //    if (new Random().Next() < CHANCE_SIN) potOps.Add(new OperatorSin());
        //    if (new Random().Next() < CHANCE_COS) potOps.Add(new OperatorCos());
        //    if (new Random().Next() < CHANCE_TAN) potOps.Add(new OperatorTan());
        //    if (new Random().Next() < CHANCE_POW) potOps.Add(new OperatorPow());
        //    Operator newOp = (potOps.Count > 0 ? potOps.get((int)(Math.random() * potOps.size())) : new OperatorAdd());

        //    Operand rhs;
        //    if (Math.random() < 0.5)
        //    {
        //        // rhs is constant

        //        double rhsVal;

        //        // Protection against math errors
        //        switch (newOp.getType())
        //        {
        //            case OPERATOR_DIV:
        //                do
        //                { // N / 0
        //                    rhsVal = MIN_VAL + Math.random() * (MAX_VAL - MIN_VAL);
        //                } while (round2(rhsVal, decimalPlaces) == 0);

        //                break;
        //            case OPERATOR_POW:
        //                if (expression.getValueAt(randTreeIndex).getType() == ExpressionType.CONSTANT && ((Constant)expression.getValueAt(randTreeIndex)).getValue() == 0) // 0 ^ -N
        //                    rhsVal = Math.abs(MIN_VAL + Math.random() * (MAX_VAL - MIN_VAL));
        //                else
        //                    rhsVal = MIN_VAL + Math.random() * (MAX_VAL - MIN_VAL);

        //                break;
        //            default:
        //                rhsVal = MIN_VAL + Math.random() * (MAX_VAL - MIN_VAL);
        //        }

        //        rhsVal = round2(rhsVal, decimalPlaces);
        //        rhs = new Constant(rhsVal);
        //    }
        //    else
        //    {
        //        // rhs is a variable
        //        rhs = new Variable((int)(Math.random() * numVars));
        //    }


        //    expression.setTreeAt
        //    (
        //        randTreeIndex,
        //        (
        //            newOp.isUnary()
        //            ?
        //            new BinaryTree<ExpressionPart>(newOp, expression.getTreeAt(randTreeIndex))
        //            :
        //            new BinaryTree<ExpressionPart>(newOp, expression.getTreeAt(randTreeIndex), rhs)
        //        )
        //    );
        //}

        //public void expand(int times)
        //{
        //    for (int i = 0; i < times; i++)
        //        expand();
        //}

        private double Random()
        {
            return new Random().NextDouble();
        }

        public void addAllVars()
        {
            for (int i = 0; i < numVars; i++)
            {
                List<Operator> potOps = new List<Operator>();
                if (Random() < CHANCE_ADD) potOps.Add(new OperatorAdd());
                //if (Random() < CHANCE_SUB) potOps.Add(new OperatorSubtract());
                if (Random() < CHANCE_MULT) potOps.Add(new OperatorMultiply());
                if (Random() < CHANCE_DIV) potOps.Add(new OperatorDivide());
                Operator newOp = (potOps.Count > 0 ? potOps[((int)(Random() * potOps.Count))] : new OperatorAdd());
                Operand rhs = new Variable(i);

                int randTreeIndex = (int)(Random() * expression.size());
                expression.setTreeAt
                (
                    randTreeIndex,
                    new BinaryTree<ExpressionPart>(newOp, expression.getTreeAt(randTreeIndex), rhs)
                );
            }
        }

        // Set chances to have these in an expression
        public void setAddChance(double chance) { CHANCE_ADD = chance; }
        public void setSubChance(double chance) { CHANCE_SUB = chance; }
        public void setMultChance(double chance) { CHANCE_MULT = chance; }
        public void setDivChance(double chance) { CHANCE_DIV = chance; }
        public void setPowChance(double chance) { CHANCE_POW = chance; }
        public void setSinChance(double chance) { CHANCE_SIN = chance; }
        public void setCosChance(double chance) { CHANCE_COS = chance; }
        public void setTanChance(double chance) { CHANCE_TAN = chance; }

        // All-in-one convenience function
        public void setChances(double c_add, double c_sub, double c_mult, double c_div, double c_pow, double c_sin, double c_cos, double c_tan)
        {
            CHANCE_ADD = c_add;
            CHANCE_SUB = c_sub;
            CHANCE_MULT = c_mult;
            CHANCE_DIV = c_div;
            CHANCE_POW = c_pow;
            CHANCE_SIN = c_sin;
            CHANCE_COS = c_cos;
            CHANCE_TAN = c_tan;
        }

        //

        public void simplify()
        {
            for (int i = 0; i < expression.size(); i++)
            {
                BinaryTree<ExpressionPart> ct = expression.getTreeAt(i);

                //System.out.println(getInfixExpression(ct));

                BinaryTree<ExpressionPart> prevExpression = new BinaryTree<ExpressionPart>(ct);

                switch (ct.getRoot().getType())
                {
                    case ExpressionType.OPERATOR_ADD:
                        if (ct.getLeft().getRoot().getType() == ExpressionType.OPERATOR_SUB)
                        {
                            if (ct.getLeft().getRight().equals(ct.getRight()))
                                // (N - E) + E, ROOT = ROOT.LEFT.LEFT
                                expression.setTreeAt(i, new BinaryTree<ExpressionPart>(ct.getLeft().getLeft()));
                        }
                        else if (ct.getRight().getRoot().getType() == ExpressionType.OPERATOR_SUB)
                        {
                            if (ct.getLeft().equals(ct.getRight().getRight()))
                                // E + (N - E), ROOT = ROOT.RIGHT.LEFT
                                expression.setTreeAt(i, new BinaryTree<ExpressionPart>(ct.getRight().getLeft()));
                        }
                        else if (ct.getLeft().equals(ct.getRight()))
                        {
                            BinaryTree<ExpressionPart> newTree = new BinaryTree<ExpressionPart>(
                                new OperatorMultiply(),
                                new BinaryTree<ExpressionPart>(new Constant(2)),
                                new BinaryTree<ExpressionPart>(ct.getRight())
                            );

                            // E + E, ROOT = TREE(MULT, 2, ROOT.RIGHT)
                            expression.setTreeAt(i, newTree);
                        }
                        else if (ct.getLeft().getRoot().getType() == ExpressionType.CONSTANT && ct.getRight().getRoot().getType() == ExpressionType.CONSTANT)
                        {
                            // C + C, ROOT = ROOT.LEFT + ROOT.RIGHT
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(new Constant(((Constant)ct.getLeft().getRoot()).getValue() + ((Constant)ct.getRight().getRoot()).getValue())));
                        }
                        else if (ct.getLeft().getRoot().getType() == ExpressionType.CONSTANT && ((Constant)ct.getLeft().getRoot()).getValue() == 0)
                        {
                            // 0 + N, ROOT = ROOT.RIGHT
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(ct.getRight()));
                        }
                        else if (ct.getRight().getRoot().getType() == ExpressionType.CONSTANT && ((Constant)ct.getRight().getRoot()).getValue() == 0)
                        {
                            // N + 0, ROOT = ROOT.LEFT
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(ct.getLeft()));
                        }
                        break;
                    case ExpressionType.OPERATOR_SUB:
                        if (ct.getLeft().getRoot().getType() == ExpressionType.OPERATOR_ADD)
                        {
                            if (ct.getLeft().getLeft().equals(ct.getRight()))
                                // (N + E) - E, ROOT = ROOT.LEFT.RIGHT
                                expression.setTreeAt(i, new BinaryTree<ExpressionPart>(ct.getLeft().getRight()));
                            else if (ct.getLeft().getRight().equals(ct.getRight()))
                                // (E + N) - E, ROOT = ROOT.LEFT.LEFT
                                expression.setTreeAt(i, new BinaryTree<ExpressionPart>(ct.getLeft().getLeft()));
                        }
                        else if (ct.getLeft().equals(ct.getRight()))
                        {
                            // E - E, ROOT = 0
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(new Constant(0)));
                        }
                        else if (ct.getLeft().getRoot().getType() == ExpressionType.CONSTANT && ct.getRight().getRoot().getType() == ExpressionType.CONSTANT)
                        {
                            // C - C, ROOT = ROOT.LEFT - ROOT.RIGHT
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(new Constant(((Constant)ct.getLeft().getRoot()).getValue() - ((Constant)ct.getRight().getRoot()).getValue())));
                        }
                        else if (ct.getRight().getRoot().getType() == ExpressionType.CONSTANT && ((Constant)ct.getRight().getRoot()).getValue() == 0)
                        {
                            // N - 0, ROOT = ROOT.LEFT
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(ct.getLeft()));
                        }
                        break;
                    case ExpressionType.OPERATOR_MULT:
                        if (ct.getLeft().getRoot().getType() == ExpressionType.OPERATOR_DIV)
                        {
                            if (ct.getRight().equals(ct.getLeft().getRight()))
                                // (N / E) * E, ROOT = ROOT.LEFT.LEFT
                                expression.setTreeAt(i, new BinaryTree<ExpressionPart>(ct.getLeft().getLeft()));
                        }
                        else if (ct.getRight().getRoot().getType() == ExpressionType.OPERATOR_DIV)
                        {
                            if (ct.getLeft().equals(ct.getRight().getRight()))
                                // E * (N / E), ROOT = ROOT.RIGHT.LEFT
                                expression.setTreeAt(i, new BinaryTree<ExpressionPart>(ct.getRight().getLeft()));
                        }
                        else if (ct.getLeft().equals(ct.getRight()))
                        {
                            BinaryTree<ExpressionPart> newTree = new BinaryTree<ExpressionPart>(
                                new OperatorPow(),
                                new BinaryTree<ExpressionPart>(ct.getLeft()),
                                new BinaryTree<ExpressionPart>(new Constant(2))
                            );

                            // E * E, ROOT = TREE(POW, ROOT.LEFT, 2)
                            expression.setTreeAt(i, newTree);
                        }
                        else if (ct.getLeft().getRoot().getType() == ExpressionType.CONSTANT && ct.getRight().getRoot().getType() == ExpressionType.CONSTANT)
                        {
                            // C * C, ROOT = ROOT.LEFT * ROOT.RIGHT
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(new Constant(((Constant)ct.getLeft().getRoot()).getValue() * ((Constant)ct.getRight().getRoot()).getValue())));
                        }
                        else if ((ct.getLeft().getRoot().getType() == ExpressionType.CONSTANT && ((Constant)ct.getLeft().getRoot()).getValue() == 0) || (ct.getRight().getRoot().getType() == ExpressionType.CONSTANT && ((Constant)ct.getRight().getRoot()).getValue() == 0))
                        {
                            // N * 0, ROOT = 0
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(new Constant(0)));
                        }
                        else if (ct.getLeft().getRoot().getType() == ExpressionType.CONSTANT && ((Constant)ct.getLeft().getRoot()).getValue() == 1)
                        {
                            // 1 * N, ROOT = ROOT.RIGHT
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(ct.getRight()));
                        }
                        else if (ct.getRight().getRoot().getType() == ExpressionType.CONSTANT && ((Constant)ct.getRight().getRoot()).getValue() == 1)
                        {
                            // N * 1, ROOT = ROOT.LEFT
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(ct.getLeft()));
                        }
                        break;
                    case ExpressionType.OPERATOR_DIV:
                        if (ct.getLeft().equals(ct.getRight()))
                        {
                            // E / E, ROOT = 1
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(new Constant(1)));
                        }
                        else if (ct.getLeft().getRoot().getType() == ExpressionType.CONSTANT && ct.getRight().getRoot().getType() == ExpressionType.CONSTANT)
                        {
                            // C / C, ROOT = ROOT.LEFT / ROOT.RIGHT
                            double result = ((Constant)ct.getLeft().getRoot()).getValue() / ((Constant)ct.getRight().getRoot()).getValue();
                            if (!Double.IsNaN(result))
                                expression.setTreeAt(i, new BinaryTree<ExpressionPart>(new Constant(result)));
                        }
                        else if (ct.getRight().getRoot().getType() == ExpressionType.CONSTANT && ((Constant)ct.getRight().getRoot()).getValue() == 1)
                        {
                            // N / 1, ROOT = ROOT.LEFT
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(ct.getLeft()));
                        }
                        else if (ct.getLeft().getRoot().getType() == ExpressionType.CONSTANT && ((Constant)ct.getLeft().getRoot()).getValue() == 0)
                        {
                            // 0 / N, ROOT = 0
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(new Constant(0)));
                        }
                        break;
                    case ExpressionType.OPERATOR_POW:
                        if (ct.getLeft().getRoot().getType() == ExpressionType.CONSTANT && ct.getRight().getRoot().getType() == ExpressionType.CONSTANT)
                        {
                            // C ^ C, ROOT = ROOT.LEFT ^ ROOT.RIGHT
                            double result = Math.Pow(((Constant)ct.getLeft().getRoot()).getValue(), ((Constant)ct.getRight().getRoot()).getValue());
                            if (!Double.IsNaN(result))
                                expression.setTreeAt(i, new BinaryTree<ExpressionPart>(new Constant(result)));
                        }
                        else if (ct.getRight().getRoot().getType() == ExpressionType.CONSTANT && ((Constant)ct.getRight().getRoot()).getValue() == 0)
                        {
                            // N ^ 0, ROOT = 1
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(new Constant(1)));
                        }
                        else if (ct.getRight().getRoot().getType() == ExpressionType.CONSTANT && ((Constant)ct.getRight().getRoot()).getValue() == 1)
                        {
                            // N ^ 1, ROOT = ROOT.LEFT
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(ct.getLeft()));
                        }
                        else if (ct.getLeft().getRoot().getType() == ExpressionType.CONSTANT && ((Constant)ct.getLeft().getRoot()).getValue() == 1)
                        {
                            // 1 ^ N, ROOT = 1
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(new Constant(1)));
                        }
                        else if (ct.getLeft().getRoot().getType() == ExpressionType.CONSTANT && ((Constant)ct.getLeft().getRoot()).getValue() == 0)
                        {
                            // 0 ^ N, ROOT = 0
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(new Constant(0)));
                        }
                        break;
                    case ExpressionType.OPERATOR_SIN:
                        if (ct.getLeft().getRoot().getType() == ExpressionType.CONSTANT)
                        {
                            // SIN(C), ROOT = SIN(ROOT.LEFT)
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(new Constant(Math.Sin(((Constant)ct.getLeft().getRoot()).getValue()))));
                        }
                        break;
                    case ExpressionType.OPERATOR_COS:
                        if (ct.getLeft().getRoot().getType() == ExpressionType.CONSTANT)
                        {
                            // COS(C), ROOT = COS(ROOT.LEFT)
                            expression.setTreeAt(i, new BinaryTree<ExpressionPart>(new Constant(Math.Cos(((Constant)ct.getLeft().getRoot()).getValue()))));
                        }
                        break;
                    case ExpressionType.OPERATOR_TAN:
                        if (ct.getLeft().getRoot().getType() == ExpressionType.CONSTANT)
                        {
                            // TAN(C), ROOT = TAN(ROOT.LEFT)
                            double result = Math.Tan(((Constant)ct.getLeft().getRoot()).getValue());
                            if (!Double.IsNaN(result))
                                expression.setTreeAt(i, new BinaryTree<ExpressionPart>(new Constant(result)));
                        }
                        break;
                }

                // If the tree was changed, restart the for loop.
                i = !prevExpression.equals(expression.getTreeAt(i)) ? 0 : i;
            }
        }

        //

        public String toString()
        {
            String result = getInfixExpression(expression);

            // Remove surrounding parenthesis if applicable
            if (result.startsWith("(") && result.endsWith(")"))
                result = result.substring(1, result.length() - 1);

            return result;
        }

        private String getInfixExpression(BinaryTree<ExpressionPart> tree)
        {
            String result = "";

            if (!tree.isEmpty())
            {
                if (tree.getRoot().isOperand())
                    result += tree.getRoot();
                else
                {
                    if (((Operator)tree.getRoot()).isUnary())
                    {
                        result += tree.getRoot() + "(";
                        result += getInfixExpression(tree.getLeft());
                        result += ")";
                    }
                    else
                    {
                        result += "(";
                        result += getInfixExpression(tree.getLeft());
                        result += " " + tree.getRoot() + " ";
                        result += getInfixExpression(tree.getRight());
                        result += ")";
                    }
                }
            }

            return result;
        }

        public String toPrefixString()
        {
            return "" + expression;
        }

        private static double round2(double val, int places)
        {
            if (places >= 0)
                return Math.Round(val * Math.Pow(10, places)) / Math.Pow(10, places);
            else
                return val;
        }

        public void setDecimalPlaces(int places)
        {
            decimalPlaces = places;
        }

        public int getTreeSize()
        {
            return expression.size();
        }
    }

}
