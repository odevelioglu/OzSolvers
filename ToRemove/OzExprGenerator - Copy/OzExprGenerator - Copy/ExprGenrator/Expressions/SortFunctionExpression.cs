using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprGenrator
{
    public class SortFunctionExpression : IExpression
    {
        public List<Type> ParameterTypes => new List<Type> { typeof(List<int>) };

        public Type ReturnType => typeof(List<int>);

        public IEnumerable<BaseFunction> GenerateDepthFirst(Context context, BaseFunction parentFunc)
        {
            var sort = new SortFunction { Parent = parentFunc };

            sort.ParamValues[0] = context.List;

            sort.ParamValues[1] = new EmptyFunction();
            yield return sort;
            sort.ParamValues[1] = null;

            foreach (var block in context.Block.GenerateDepthFirst(context, sort))
            {
                sort.ParamValues[1] = block;
                yield return sort;
                sort.ParamValues[1] = null;
            }

            sort.ParamValues[0] = null;
        }

        public IEnumerable<BaseFunction> GenerateDepthFirstDyna(Context context, BaseFunction parentFunc)
        {
            var sort = new SortFunction { Parent = parentFunc };

            sort.ParamValues[0] = context.List;

            sort.ParamValues[1] = new EmptyFunction();
            yield return sort;
            sort.ParamValues[1] = null;

            foreach (var block in context.Block.GenerateDepthFirstDyna(context, sort))
            {
                sort.ParamValues[1] = block;
                yield return sort;
                sort.ParamValues[1] = null;
            }

            sort.ParamValues[0] = null;
        }
    }

    public class SortFunction : BaseFunction
    {
        public override string Name => "sort";
        public SortFunction()
        {
            ParamValues = new BaseFunction[2];
        }

        public override bool IsValid()
        {
            return true;
        }

        public override object Eval()
        {
            ExecutionCounter.Evaluated?.Invoke(this);

            ParamValues[1].Eval();
            return ParamValues[0].Eval();
        }

        public override string ToString()
        {
            return "sort(A)" + "{" + ParamValues[1] + "}";
        }

        public override BaseFunction Copy(BaseFunction parent)
        {
            var copy = new SortFunction { Parent = parent };
            copy.ParamValues = new[] { ParamValues[0].Copy(copy), ParamValues[1].Copy(copy) };
            return copy;
        }
    }
}
