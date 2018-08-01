using System;
using System.Collections.Generic;
using System.Linq;

namespace ExprGenrator
{
    public class IfFunctionExpression : IExpression
    {
        public int Max { get; } = int.MaxValue;
        public IfFunctionExpression(int max)
        {
            this.Max = max;
        }

        public List<Type> ParameterTypes => new List<Type> { typeof(bool), typeof(IExpression) };

        public Type ReturnType => typeof(void);
        public static Dictionary<string, BaseFunction[]> Cache = new Dictionary<string, BaseFunction[]>();

        public IEnumerable<BaseFunction> GenerateDepthFirst(Context context, BaseFunction parentFunc)
        {
            if (parentFunc.Count("if") >= this.Max) yield break;

            if (!parentFunc.CheckDepth(context))
                yield break;

            var maxSwap = context.VoidExpressions.OfType<SwapFunctionExpression>().Single().Max;
            var swap = parentFunc.Count("swap");
            if (swap >= maxSwap) yield break;

            var func = new IfFunction { Parent = parentFunc };

            foreach (var voidFuncIfTrue in context.Block.GenerateDepthFirst(context, func))
            {
                func.ParamValues[1] = voidFuncIfTrue;

                foreach (var boolFunc in context.BoolExpressions.SelectMany(p => p.GenerateDepthFirst(context, func)))
                {
                    func.ParamValues[0] = boolFunc;

                    if (func.IsValid())
                        yield return func;

                    func.ParamValues[0] = null;
                }

                func.ParamValues[1] = null;
            }
        }

        public IEnumerable<BaseFunction> GenerateDepthFirstDyna(Context context, BaseFunction parentFunc)
        {
            if (parentFunc.Count("if") >= this.Max) yield break;

            if (!parentFunc.CheckDepth(context))
                yield break;

            var maxIff = context.VoidExpressions.OfType<IfFunctionExpression>().Single().Max;
            var iff = parentFunc.Count("if");

            var maxSwap = context.VoidExpressions.OfType<SwapFunctionExpression>().Single().Max;
            var swap = parentFunc.Count("swap");
            if (swap >= maxSwap) yield break;

            var maxBlock = ((BlockFunctionExpression) context.Block).Max;
            var block = parentFunc.Count("block");

            var ifDepthmax = ((ListConstant)context.List).Value.Length - 1;
            var key = "block:" + (maxBlock - block) + " if:" + (maxIff - iff) + " swap:" + (maxSwap - swap) + " ifDepth:" + (ifDepthmax - parentFunc.GetIfDepth(context));
            //var key = parentFunc.UniqueKey();
            
            if (!Cache.ContainsKey(key))
            {
                var list = new List<BaseFunction>();
                var func = new IfFunction { Parent = parentFunc };

                foreach (var voidFuncIfTrue in context.Block.GenerateDepthFirstDyna(context, func))
                {
                    func.ParamValues[1] = voidFuncIfTrue;

                    foreach (var boolFunc in context.BoolExpressions.SelectMany(p => p.GenerateDepthFirstDyna(context, func)))
                    {
                        func.ParamValues[0] = boolFunc;

                        if (func.IsValid())
                        {
                            var clone = (int[])((ListConstant)context.List).Value.Clone();
                            if (func.TestState(clone, context))
                            {
                                if (func.CheckDepth(context))
                                    list.Add(func.Copy(null));
                            }
                        }

                        func.ParamValues[0] = null;
                    }

                    func.ParamValues[1] = null;
                }

                Cache[key] = list.ToArray();
            }

            foreach (var f in Cache[key])
            {
                f.Parent = parentFunc;
                yield return f;
            }
        }
    }

    public class IfFunction : BaseFunction
    {
        public override string Name => "if";

        public IfFunction()
        {
            ParamValues = new BaseFunction[2];
        }

        public override bool IsValid()
        {
            var block = this.ParamValues[1] as BlockFunction;
            var currentChild = block?.ParamValues[0] as IfFunction;

            while (currentChild != null)
            {
                if (!this.CheckChild(currentChild)) return false;

                block = currentChild.ParamValues[1] as BlockFunction;
                currentChild = block?.ParamValues[0] as IfFunction;
            }

            return true;
        }

        private bool CheckChild(IfFunction ifParent)
        {
            if (ifParent == null) return true;

            var parentParams = ifParent.GetParams();
            var thisParams = this.GetParams();

            if (parentParams == null || thisParams == null) return true;

            //if(A[0] > A[1]){ if(A[0] > A[1]){ ...  } }
            if (parentParams[0] == thisParams[0] && parentParams[1] == thisParams[1])
                return false;

            //if(A[0] > A[1]){ if(A[1] > A[0]){ ...  } }
            if (parentParams[0] == thisParams[1] && parentParams[1] == thisParams[0])
                return false;

            return true;
        }

        public override object Eval()
        {
            ExecutionCounter.Evaluated?.Invoke(this);

            if ((bool)ParamValues[0].Eval())
            {
                ParamValues[1].Eval();
            }

            return null;
        }

        public override BaseFunction Copy(BaseFunction parent)
        {
            var copy = new IfFunction { Parent = parent };
            copy.ParamValues = new[] { ParamValues[0].Copy(copy), ParamValues[1].Copy(copy) };
            return copy;
        }

        public override string ToString()
        {
            return $"if({ParamValues[0]})" + "{ " + $"{ParamValues[1]}" + " }";
        }
    }
}
