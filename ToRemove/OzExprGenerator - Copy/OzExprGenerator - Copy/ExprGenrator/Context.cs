using System.Collections.Generic;
using System.Linq;

namespace ExprGenrator
{
    public class Context
    {
        private List<IExpression> Expressions = new List<IExpression>();
        public List<BaseFunction> Ints = new List<BaseFunction>();
        public BaseFunction List;

        public IExpression Block;

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
            context.Block = new BlockFunctionExpression(MathTools.Fact(theList.Length));
            
            context.AddExpression(new SwapFunctionExpression(theList.Length * (theList.Length - 1) / 2));
            context.AddExpression(new IfFunctionExpression(theList.Length * (theList.Length - 1) / 2));
            context.AddExpression(new GreaterThanFunctionExpression());
            context.AddExpression(new GetValueFunctionExpression());

            context.List = new ListConstant(theList);
            context.Ints.AddRange(Enumerable.Range(0, theList.Length).Select(p => new IntConstant(p)));

            BlockFunctionExpression.Cache = new Dictionary<string, BaseFunction[]>();
            SwapFunctionExpression.Cache = null;
            IfFunctionExpression.Cache = new Dictionary<string, BaseFunction[]>();
            GreaterThanFunctionExpression.Cache = null;
            return context;
        }
    }
}
