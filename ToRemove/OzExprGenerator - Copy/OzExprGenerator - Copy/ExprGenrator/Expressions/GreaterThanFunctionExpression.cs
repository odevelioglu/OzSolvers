using System;
using System.Collections.Generic;
using System.Linq;

namespace ExprGenrator
{
    public class GreaterThanFunctionExpression : IExpression
    {
        public List<Type> ParameterTypes => new List<Type> { typeof(int), typeof(int) };

        public Type ReturnType => typeof(bool);

        public static BaseFunction[] Cache;

        public IEnumerable<BaseFunction> GenerateDepthFirst(Context context, BaseFunction parentFunc)
        {
            var func = new GreaterThanFunction { Parent = parentFunc };

            var ind = 0;
            foreach (var intVar0 in context.IntExpressions.SelectMany(p => p.GenerateDepthFirst(context, func)))
            {
                ind++;
                func.ParamValues[0] = intVar0;
                foreach (var intVar1 in context.IntExpressions.SelectMany(p => p.GenerateDepthFirst(context, func)).Skip(ind))
                {
                    func.ParamValues[1] = intVar1;

                    yield return func;
                }
            }
        }

        public IEnumerable<BaseFunction> GenerateDepthFirstDyna(Context context, BaseFunction parentFunc)
        {
            if (Cache == null)
            {
                var list = new List<BaseFunction>();

                var intFunctions = context.IntExpressions
                    .SelectMany(p => p.GenerateDepthFirstDyna(context, null))
                    .ToArray();

                var ind = 0;
                foreach (var intVar0 in intFunctions)
                {
                    ind++;
                    foreach (var intVar1 in intFunctions.Skip(ind))
                    {
                        list.Add(new GreaterThanFunction { Parent = parentFunc, ParamValues = new[] { intVar0, intVar1 } });
                    }
                }

                Cache = list.ToArray();
            }

            foreach (var val in Cache)
            {
                val.Parent = parentFunc;
                yield return val;
            }
        }
    }

    public class GreaterThanFunction : BaseFunction
    {
        public override string Name => "gt";
        public GreaterThanFunction()
        {
            ParamValues = new BaseFunction[2];
        }

        public override bool IsValid()
        {
            if (((IntConstant)ParamValues[0].ParamValues[1]).Value == ((IntConstant)ParamValues[1].ParamValues[1]).Value)
                return false;

            return true;
        }

        public override object Eval()
        {
            ExecutionCounter.Evaluated?.Invoke(this);
            return (int)ParamValues[0].Eval() > (int)ParamValues[1].Eval();
        }

        public override string ToString()
        {
            return $"{ParamValues[0]} > {ParamValues[1]}";
        }

        public override BaseFunction Copy(BaseFunction parent)
        {
            var copy = new GreaterThanFunction { Parent = parent };
            copy.ParamValues = new[] { ParamValues[0].Copy(copy), ParamValues[1].Copy(copy) };
            return copy;
        }
    }
}
