using System;
using System.Collections.Generic;
using System.Linq;

namespace ExprGenrator
{
    public class Context
    {
        private List<IExpression> Expressions = new List<IExpression>();
        public List<BaseFunction> Ints = new List<BaseFunction>();
        public BaseFunction List;
        
        public void AddExpression(IExpression expr)
        {
            this.Expressions.Add(expr);

            this.VoidExpressions = this.Expressions.Where(p => p.ReturnType == typeof(void)).ToArray();
            this.IntExpressions = this.Expressions.Where(p => p.ReturnType == typeof(int)).ToArray();
            this.BoolExpressions = this.Expressions.Where(p => p.ReturnType == typeof(bool)).ToArray();
        }

        public IExpression[] VoidExpressions;
        public IExpression[] IntExpressions;
        public IExpression[] BoolExpressions;

        public static Context CreateDefault(int[] theList)
        {
            var context = new Context();

            var maxSwap = theList.Length * (theList.Length - 1) / 2; // should be 7 for list of 3
            context.AddExpression(new SwapFunctionExpression(maxSwap));

            var maxIf = Convert.ToInt32(Math.Ceiling(Math.Log(MathTools.Fact(theList.Length), 2)));
            var ifExpr = new IfFunctionExpression(maxIf);
            
            switch (theList.Length)
            {
                case 1:
                    ifExpr.MaxDepth = 0; 
                    ifExpr.MaxExec = 0;
                    break;
                case 2:
                    ifExpr.MaxDepth = 1;
                    ifExpr.MaxExec = 2; //log(2)/log(2)*2 = 2
                    break;
                case 3:
                    ifExpr.MaxDepth = 2;
                    ifExpr.MaxExec = 16; //log(6)/log(2)*6=15.5
                    break;
                case 4:
                    ifExpr.MaxDepth = 2;
                    ifExpr.MaxExec = 112; // log(24)/log(2)*24=110.04
                    break;
                case 5:
                    ifExpr.MaxDepth = 3; //??
                    ifExpr.MaxExec = 7*120; //log(120)/log(2)*120=828.8
                    break; 
            }

            context.AddExpression(ifExpr);
            //context.AddExpression(new IfElseFunctionExpression(Convert.ToInt32(Math.Pow(2, maxIf)) - 1)); //??

            context.AddExpression(new GreaterThanFunctionExpression());
            context.AddExpression(new GetValueFunctionExpression());

            context.List = new ListConstant(theList);
            context.Ints.AddRange(Enumerable.Range(0, theList.Length).Select(p => new IntConstant(p)));

            SwapFunctionExpression.ParamCache = null;
            GreaterThanFunctionExpression.Cache = null;
            return context;
        }
    }
}
