using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprGenrator
{
    public class SwapFunctionExpression : IExpression
    {
        public int Max { get; } = int.MaxValue;
        public SwapFunctionExpression(int max)
        {
            this.Max = max;
        }
        public List<Type> ParameterTypes => new List<Type> { typeof(List<int>), typeof(int), typeof(int) };
        public Type ReturnType => typeof(void);

        public static BaseFunction[] Cache; // has always 3 elems
        public IEnumerable<BaseFunction> GenerateDepthFirst(Context context, BaseFunction parentFunc)
        {
            if (parentFunc.Count("swap") >= this.Max) yield break;

            var func = new SwapFunction { Parent = parentFunc };

            func.ParamValues[0] = context.List;

            var ind = 0;
            foreach (var intVar0 in context.Ints)
            {
                ind++;
                func.ParamValues[1] = intVar0;

                foreach (var intVar1 in context.Ints.Skip(ind))
                {
                    func.ParamValues[2] = intVar1;

                    yield return func;

                    func.ParamValues[2] = null;
                }

                func.ParamValues[1] = null;
            }
        }

        public IEnumerable<BaseFunction> GenerateDepthFirstDyna(Context context, BaseFunction parentFunc)
        {
            if (parentFunc.Count("swap") >= this.Max) yield break;

            if (Cache == null)
            {
                Cache = this.GenerateDepthFirst(context, parentFunc).Select(p => p.Copy(null)).ToArray();
            }

            foreach (var val in Cache)
            {
                val.Parent = parentFunc;
                yield return val;
            }
        }
    }

    public class SwapFunction : BaseFunction
    {
        public override string Name => "swap";
        public SwapFunction()
        {
            ParamValues = new BaseFunction[3];
        }

        public override bool IsValid()
        {
            //if (ParamValues[1].Eval().Equals(ParamValues[2].Eval())) return false;
            return true;
        }

        public override object Eval()
        {
            ExecutionCounter.Evaluated?.Invoke(this);

            var list = (int[])ParamValues[0].Eval();
            var index0 = (int)ParamValues[1].Eval();
            var index1 = (int)ParamValues[2].Eval();

            var tmp = list[index0];
            list[index0] = list[index1];
            list[index1] = tmp;

            StateTester.AfterEval?.Invoke(this, list);

            return null;
        }

        public override string ToString()
        {
            return $"swap(A, {ParamValues[1]}, {ParamValues[2]})";
        }

        public override BaseFunction Copy(BaseFunction parent)
        {
            var copy = new SwapFunction { Parent = parent };
            copy.ParamValues = new[] { ParamValues[0].Copy(copy), ParamValues[1].Copy(copy), ParamValues[2].Copy(copy) };
            return copy;
        }
    }
}
