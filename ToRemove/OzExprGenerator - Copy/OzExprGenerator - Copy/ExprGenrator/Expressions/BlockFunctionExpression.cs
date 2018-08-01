using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprGenrator
{
    public class BlockFunctionExpression : IExpression
    {
        public int Max { get; } = int.MaxValue;
        public BlockFunctionExpression(int max)
        {
            this.Max = max;
        }
        public List<Type> ParameterTypes => new List<Type> { typeof(List<BaseFunction>) };
        public Type ReturnType => typeof(void);

        public IEnumerable<BaseFunction> GenerateDepthFirst(Context context, BaseFunction parentFunc)
        {
            if (parentFunc.Count("block") >= this.Max) yield break;

            if (!parentFunc.CheckDepth(context))
                yield break;

            var maxSwap = context.VoidExpressions.OfType<SwapFunctionExpression>().Single().Max;
            var swap = parentFunc.Count("swap");
            if (swap >= maxSwap) yield break;

            var func = new BlockFunction { Parent = parentFunc };

            foreach (var voidExpression in context.VoidExpressions)
            { 
                foreach (var voidFunc in voidExpression.GenerateDepthFirst(context, func))
                {
                    func.ParamValues[0] = voidFunc;

                    func.ParamValues[1] = new EmptyFunction();
                    if (func.IsValid())
                    {
                        yield return func;
                    }
                    func.ParamValues[1] = null;

                    foreach (var intVar1 in this.GenerateDepthFirst(context, func))
                    {
                        func.ParamValues[1] = intVar1;

                        if (func.IsValid())
                            yield return func;

                        func.ParamValues[1] = null;
                    }

                    func.ParamValues[0] = null;
                }
            }
        }

        public static Dictionary<string, BaseFunction[]> Cache = new Dictionary<string, BaseFunction[]>();
        public IEnumerable<BaseFunction> GenerateDepthFirstDyna(Context context, BaseFunction parentFunc)
        {
            if (parentFunc.Count("block") >= this.Max) yield break;

            if (!parentFunc.CheckDepth(context))
                yield break;

            //var key = parentFunc.UniqueKey();

            //if (!Cache.ContainsKey(key))
            {
                //var list = new List<BaseFunction>();
                var func = new BlockFunction { Parent = parentFunc };

                foreach (var voidFunc in context.VoidExpressions.SelectMany(p => p.GenerateDepthFirstDyna(context, func)))
                {
                    func.ParamValues[0] = voidFunc;

                    func.ParamValues[1] = new EmptyFunction();
                    if (func.IsValid())
                    {
                        yield return func;
                        //list.Add(func.Copy(parentFunc));
                    }
                    func.ParamValues[1] = null;

                    foreach (var blockFunc in this.GenerateDepthFirstDyna(context, func))
                    {
                        func.ParamValues[1] = blockFunc;

                        if (func.IsValid())
                        {
                            yield return func;
                            //list.Add(func.Copy(parentFunc));
                        }

                        func.ParamValues[1] = null;
                    }

                    func.ParamValues[0] = null;
                }

                //Cache[key] = list.ToArray();
            }

            //foreach (var f in Cache[key])
            //{
            //    f.Parent = parentFunc;
            //    yield return f;
            //}
        }

        //public IExpression[] GetGeneratableExpression(Context context, BaseFunction parentFunc)
        //{
        //    if (parentFunc.Count<BlockFunction>() >= this.Max) return new IExpression[]{};

        //    var voids = context.VoidExpressions.ToList();
        //    if (!CheckDepth(parentFunc) || parentFunc.Count<IfFunction>() >= 6)
        //        voids = voids.Where(p => p.GetType() != typeof(IfFunctionExpression)).ToList();

        //    if (parentFunc.Count<SwapFunction>() >= 6)
        //        voids = voids.Where(p => p.GetType() != typeof(SwapFunctionExpression)).ToList();

        //    return voids.ToArray();
        //}

    }

    public class BlockFunction : BaseFunction
    {
        public override string Name => "block";
        public BlockFunction()
        {
            ParamValues = new BaseFunction[2];
        }

        public override bool IsValid()
        {
            if (this.ParamValues[0] is IfFunction fristIf && this.ParamValues[1] is BlockFunction secondblock)
            {
                //block(if(A[0] > A[1]){ swap(A, 0, 1);  }; block(if(A[0] > A[1]){ swap(A, 0, 1);  })) }
                if (secondblock.ParamValues[0] is IfFunction secondIf)
                {
                    var fristparams = fristIf.GetParams();
                    var secondParams = secondIf.GetParams();

                    if (fristparams[0] == secondParams[0] && fristparams[1] == secondParams[1])
                        return false;

                    //if (fristparams[0] == secondParams[1] && fristparams[1] == secondParams[0])
                    //    return false;
                }
            }

            // SORT specific: No two swap following 
            if (this.ParamValues[0] is SwapFunction && (ParamValues[1] as BlockFunction)?.ParamValues[0] is SwapFunction)
            {
                return false;
            }

            // SORT specific: 
            if (this.ParamValues[0] is SwapFunction swap && (ParamValues[1] as BlockFunction)?.ParamValues[0] is IfFunction iff)
            {
                var ifparams = iff.GetParams();
                if (((IntConstant)swap.ParamValues[1]).Value == ifparams[0] && ((IntConstant)swap.ParamValues[2]).Value == ifparams[1])
                    return false;
            }

            // SORT specific: 
            //if (this.ParamValues[0] is SwapFunction swap1 && (ParamValues[1] as BlockFunction)?.ParamValues[1] is BlockFunction block2)
            //{
            //    if (block2.ParamValues[0] is SwapFunction swap2 && swap1.Equivalent(swap2))
            //        return false;
            //}

            // SORT specific: 
            if (this.ParamValues[0] is SwapFunction)
            {
                var oneLevelIf = this.Parent is IfFunction;
                var twoLevelIF = this.Parent is BlockFunction parentBlock && parentBlock.Parent is IfFunction; // good
                if (!oneLevelIf && !twoLevelIF)
                    return false;
            }


            //block(swap(A, 0, 1); , block(swap(A, 0, 1); )) 
            //if (this.ParamValues[0] is SwapFunction fristSwap && this.ParamValues[1] is BlockFunction secondSwapBlock && secondSwapBlock.ParamValues[0] is SwapFunction secondSwap)
            //{
            //    if (fristSwap.Equivalent(secondSwap)) 
            //        return false;
            //}

            //if (this.ParamValues[0] is SwapFunction topSwap && this.ParamValues[1] is BlockFunction secondIfBlock && secondIfBlock.ParamValues[0] is IfFunction subIf)
            //{
            //    var block = subIf.ParamValues[1] as BlockFunction;
            //    if (block?.ParamValues[0] is SwapFunction subSwap)
            //    {
            //        if (topSwap.Equivalent(subSwap))
            //            return false;
            //    }
            //}

            //if (this.ParamValues[0] is IfFunction topIf && topIf.ParamValues[1] is BlockFunction topBlock)
            //{
            //    if (topBlock.ParamValues[0] is SwapFunction swapOfIf && this.ParamValues[1] is BlockFunction secondBlock && secondBlock.ParamValues[0] is SwapFunction anotherSwap)
            //    {
            //        if (swapOfIf.Equivalent(anotherSwap))
            //            return false;
            //    }
            //}

            return true;
        }

        public override object Eval()
        {
            ExecutionCounter.Evaluated?.Invoke(this);

            foreach (var paramValue in ParamValues)
            {
                paramValue?.Eval();
            }

            return null;
        }

        public override string ToString()
        {
            return $"block({ParamValues[0]}, {ParamValues[1]})";
        }



        public override BaseFunction Copy(BaseFunction parent)
        {
            var copy = new BlockFunction { Parent = parent };
            copy.ParamValues = new[] { ParamValues[0].Copy(copy), ParamValues[1]?.Copy(copy) };
            return copy;
        }
    }
}
