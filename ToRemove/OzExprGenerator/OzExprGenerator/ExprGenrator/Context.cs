using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ExprGenrator
{
    public class Context
    {
        private List<IExpression> Expressions = new List<IExpression>();
        public List<BaseFunction> Ints = new List<BaseFunction>();
        public BaseFunction List;
        public List<int[]> InputStates;
        
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

        public static Context CreateDefaultForSort(int[] theList)
        {
            var context = new Context();

            context.InputStates = new List<int[]>();
            foreach (var perm in theList.Permute())
            {
                context.InputStates.Add(perm.ToArray());
            }

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

        public static Context CreateDefaultForMerge(int[] theList)
        {
            var context = new Context();

            context.InputStates = new List<int[]>();
            foreach (var perm in theList.Permute().Select(p => p.ToArray()))
            {
                if (theList.Count() == 3)
                {
                    if (perm[0] > perm[1])
                        continue;
                }
                if (theList.Count() == 4)
                {
                    if (perm[0] > perm[1] ||
                        perm[2] > perm[3])
                        continue;
                }
                if (theList.Count() == 5)
                {
                    if (perm[0] > perm[1] || perm[1] > perm[2] || // 3 + 2 or 2 + 3
                        perm[3] > perm[4])
                        continue;
                }
                if (theList.Count() == 6)
                {
                    if (perm[0] > perm[1] || perm[1] > perm[2] ||
                        perm[3] > perm[4] || perm[4] > perm[5])
                        continue;
                }
                context.InputStates.Add(perm);
            }

            var firstHalf = (int)Math.Ceiling((double)theList.Length / 2);
            var secondHalf = theList.Length - firstHalf;
            var inputCount = MathTools.Fact(theList.Length) / MathTools.Fact(firstHalf) / MathTools.Fact(secondHalf);
            Contract.Assert(context.InputStates.Count() == inputCount);
                        
            var swapExpr = new SwapFunctionExpression();
            context.AddExpression(swapExpr);

            var ifExpr = new IfFunctionExpression();            

            switch (theList.Length)
            {                
                case 3:
                    ifExpr.Max = 6;
                    ifExpr.MaxDepth = 3;
                    ifExpr.MaxExec = 5;
                    ifExpr.MaxConsecutive = 1;
                    swapExpr.Max = 6;
                    swapExpr.MaxConsecutive = 1;                    
                    break;
                case 4:
                    ifExpr.Max = 6;
                    ifExpr.MaxDepth = 2;
                    ifExpr.MaxExec = 16;
                    ifExpr.MaxConsecutive = 2;
                    swapExpr.Max = 6;
                    swapExpr.MaxConsecutive = 2;
                    break;
                case 5:
                    ifExpr.Max = 6;
                    ifExpr.MaxDepth = 4; //3
                    ifExpr.MaxExec = 37;
                    ifExpr.MaxConsecutive = 2;
                    swapExpr.Max = 8; //6
                    swapExpr.MaxConsecutive = 2;
                    break;
                case 6:
                    ifExpr.Max = 6;
                    ifExpr.MaxDepth = 2;
                    ifExpr.MaxExec = 116;
                    ifExpr.MaxConsecutive = 2;
                    swapExpr.Max = 6;
                    swapExpr.MaxConsecutive = 4;
                    break;
            }

            context.AddExpression(ifExpr);
            context.AddExpression(new GreaterThanFunctionExpression());
            context.AddExpression(new GetValueFunctionExpression());

            context.List = new ListConstant(theList);
            context.Ints.AddRange(Enumerable.Range(0, theList.Length).Select(p => new IntConstant(p)));

            SwapFunctionExpression.ParamCache = null;
            GreaterThanFunctionExpression.Cache = null;
            return context;
        }

        public static Context CreateDefaultForMergeIfElse(int[] theList)
        {
            var context = new Context();

            context.InputStates = new List<int[]>();
            foreach (var perm in theList.Permute().Select(p => p.ToArray()))
            {
                if (theList.Count() == 3)
                {
                    if (perm[0] > perm[1])
                        continue;
                }
                if (theList.Count() == 4)
                {
                    if (perm[0] > perm[1] ||
                        perm[2] > perm[3])
                        continue;
                }
                if (theList.Count() == 5)
                {
                    if (perm[0] > perm[1] || perm[1] > perm[2] || // 3 + 2 or 2 + 3
                        perm[3] > perm[4])
                        continue;
                }
                if (theList.Count() == 6)
                {
                    if (perm[0] > perm[1] || perm[1] > perm[2] ||
                        perm[3] > perm[4] || perm[4] > perm[5])
                        continue;
                }
                context.InputStates.Add(perm);
            }

            var firstHalf = (int)Math.Ceiling((double)theList.Length / 2);
            var secondHalf = theList.Length - firstHalf;
            var inputCount = MathTools.Fact(theList.Length) / MathTools.Fact(firstHalf) / MathTools.Fact(secondHalf);
            Contract.Assert(context.InputStates.Count() == inputCount);

            var swapExpr = new SwapFunctionExpression();
            context.AddExpression(swapExpr);

            var ifExpr = new IfElseFunctionExpression();

            switch (theList.Length)
            {                
                case 4:
                    ifExpr.Max = 3;
                    ifExpr.MaxDepth = 2;
                    ifExpr.MaxExec = 16;
                    ifExpr.MaxConsecutive = 2;
                    swapExpr.Max = 6;
                    swapExpr.MaxConsecutive = 2;
                    break;                
            }

            context.AddExpression(ifExpr);
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
