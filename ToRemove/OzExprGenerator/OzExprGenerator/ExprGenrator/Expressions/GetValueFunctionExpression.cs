using System;
using System.Collections.Generic;

namespace ExprGenrator
{
    public class GetValueFunctionExpression : IExpression
    {
        public List<Type> ParameterTypes => new List<Type> { typeof(List<int>), typeof(int) };

        public Type ReturnType => typeof(int);

        public IEnumerable<BaseFunction> GenerateDepthFirst(Context context, BaseFunction parentFunc)
        {
            foreach (var intVar1 in context.Ints)
            {
                yield return new GetValueFunction
                {
                    Parent = parentFunc,
                    ParamValues = new[] { context.List, intVar1 }
                }; //TODO: parents not set
            }
        }

        public IEnumerable<BaseFunction> GenerateStateFull(Context context, BaseFunction parentFunc, bool isNext)
        {
            return this.GenerateDepthFirst(context, parentFunc);
        }

        public bool CanHaveNext => false;
    }

    public class GetValueFunction : BaseFunction
    {
        public override string Name => "gv";
        public GetValueFunction()
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

            var list = ((ListConstant)ParamValues[0]).Value;
            var index = ((IntConstant)ParamValues[1]).Value;

            return list[index];
        }

        public override string ToString()
        {
            return $"A[{ParamValues[1]}]";
        }

        public override BaseFunction Copy(BaseFunction parent)
        {
            var copy = new GetValueFunction { Parent = parent };
            copy.ParamValues = new[] { ParamValues[0].Copy(copy), ParamValues[1].Copy(copy) };
            return copy;
        }
    }

}
