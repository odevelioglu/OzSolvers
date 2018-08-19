using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ExprGenrator
{
    public class IfElseFunctionExpression : IExpression
    {
        public int Max { get; set; } = int.MaxValue;
        public int MaxDepth { get; set; } = int.MaxValue;
        public int MaxExec { get; set; } = int.MaxValue;
        public int MaxConsecutive { get; set; }
        
        public List<Type> ParameterTypes => new List<Type> { typeof(bool), typeof(IExpression) };

        public Type ReturnType => typeof(void);
        
        public IEnumerable<BaseFunction> GenerateDepthFirst(Context context, BaseFunction parentFunc)
        {
            // reject(P,c)
            if (parentFunc.Info.IfCount >= this.Max) yield break;
            //if (parentFunc.Info.IfDepth >= this.MaxDepth) yield break;
            if (parentFunc.Info.SwapCount >= this.Max) yield break;

            var ifFunc = new IfFunction(parentFunc);
            ifFunc.Info.Set(parentFunc.Info);
            ifFunc.Info.IfCount++;

            foreach (var voidFuncIfTrue in context.VoidExpressions.SelectMany(p => p.GenerateDepthFirst(context, ifFunc)))      //first param
            {
                ifFunc.ParamValues[1] = voidFuncIfTrue;

                foreach (var boolFunc in context.BoolExpressions.SelectMany(p => p.GenerateDepthFirst(context, ifFunc)))
                {
                    ifFunc.ParamValues[0] = boolFunc;

                    if (!ifFunc.IsValid()) // TODO: sort specific
                    {
                        ifFunc.ParamValues[0] = null;
                        ifFunc.Info.Set(parentFunc.Info);
                        ifFunc.Info.IfCount++;
                        continue;
                    }

                    ifFunc.Info.Set(voidFuncIfTrue.Info); // voidFuncIfTrue contains alewady if
                    
                    foreach (var nextVoidFunc in context.VoidExpressions.SelectMany(p => p.GenerateDepthFirst(context, ifFunc))) // then NEXT
                    {
                        ifFunc.Next = nextVoidFunc;
                        ifFunc.Info.Set(nextVoidFunc.Info);
                        yield return ifFunc;
                        ifFunc.Info.Set(voidFuncIfTrue.Info);
                        ifFunc.Next = null;
                    }

                    yield return ifFunc; 

                    ifFunc.ParamValues[0] = null;
                    ifFunc.Next = null;
                }

                ifFunc.ParamValues[1] = null;
                ifFunc.Info.Set(parentFunc.Info);
                ifFunc.Info.IfCount++;
                ifFunc.Next = null;
            }
        }

        public IEnumerable<BaseFunction> GenerateStateFull(Context context, BaseFunction generatorFunc, bool isNext)
        {
            // reject(P,c)
            if (generatorFunc.Info.IfCount >= this.Max)
                yield break;
            //if (generatorFunc.Info.SwapCount >= this.Max) yield break; //??

            var ifFunc = isNext ? new IfElseFunction(generatorFunc.Parent) { Before = generatorFunc } : new IfElseFunction(generatorFunc);
            // Get the scope from parent. Get the states from generator
            ifFunc.Info.Set(generatorFunc.Info);
            ifFunc.Info.IfCount++;
            ifFunc.Info.IfDepth = ifFunc.Parent.Info.IfDepth + 1;
            if (ifFunc.Info.IfDepth > this.MaxDepth) yield break;

            ifFunc.Info.IfExecCount = generatorFunc.Info.IfExecCount + ifFunc.Parent.Info.Scope.Count;
            if (ifFunc.Info.IfExecCount > this.MaxExec) yield break;

            ifFunc.Info.ConsecutiveSwapCount = 1;
            if (ifFunc.Before is IfElseFunction)
            {
                if (ifFunc.Before.Info.ConsecutiveSwapCount >= MaxConsecutive)
                    yield break;

                ifFunc.Info.ConsecutiveSwapCount = ifFunc.Before.Info.ConsecutiveSwapCount + 1;
            }

            foreach (var boolFunc in context.BoolExpressions.SelectMany(p => p.GenerateStateFull(context, ifFunc, false))) // does not change info
            {
                ifFunc.ParamValues[0] = boolFunc;

                // always get the states from the generator
                var par0 = ((IntConstant)boolFunc.ParamValues[0].ParamValues[1]).Value;
                var par1 = ((IntConstant)boolFunc.ParamValues[1].ParamValues[1]).Value;

                var inScope = new List<int>();
                var outScope = new List<int>();
                foreach (var i in ifFunc.Parent.Info.Scope)
                {
                    if (generatorFunc.Info.States[i][par0] > generatorFunc.Info.States[i][par1])
                        inScope.Add(i);
                    else
                        outScope.Add(i);
                }
                
                if (inScope.Count == 0 || outScope.Count == 0) // can be true and can be false
                {
                    ifFunc.ParamValues[0] = null;
                    ifFunc.Next = null;
                    continue;
                }

                ifFunc.Info.States = generatorFunc.Info.States;
                ifFunc.Info.Scope = inScope;

                Contract.Assert(ifFunc.Info.Scope.Count < ifFunc.Parent.Info.Scope.Count);              

                // IN SCOPE
                foreach (var voidFuncIfTrue in context.VoidExpressions.OfType<SwapFunctionExpression>().SelectMany(p => p.GenerateStateFull(context, ifFunc, false))) //first param
                {
                    ifFunc.ParamValues[1] = voidFuncIfTrue;                    
                    ifFunc.Info.Set(voidFuncIfTrue.Info); // voidFuncIfTrue contains alewady if
                    ifFunc.Info.States = voidFuncIfTrue.Info.States;
                    
                    yield return ifFunc;
                    
                    ifFunc.Info.Scope = outScope;

                    // OUT SCOPE
                    foreach (var voidFuncIfFalse in context.VoidExpressions.OfType<SwapFunctionExpression>().SelectMany(p => p.GenerateStateFull(context, ifFunc, false))) //first param
                    {
                        ifFunc.ParamValues[2] = voidFuncIfFalse;
                        ifFunc.Info.Set(voidFuncIfFalse.Info);
                        ifFunc.Info.States = voidFuncIfFalse.Info.States;
                        yield return ifFunc;

                        //ifFunc.Info.Scope = ifFunc.Parent.Info.Scope; //

                        foreach (var nextVoidFunc in context.VoidExpressions.OfType<IfElseFunctionExpression>().SelectMany(p => p.GenerateStateFull(context, ifFunc, true))) // then NEXT
                        {
                            ifFunc.Next = nextVoidFunc;
                            ifFunc.Info.Set(nextVoidFunc.Info);
                            ifFunc.Info.States = nextVoidFunc.Info.States;

                            yield return ifFunc;

                            ifFunc.Next = null;
                            ifFunc.Info.Set(voidFuncIfTrue.Info);
                            ifFunc.Info.States = voidFuncIfTrue.Info.States;
                        }

                        ifFunc.ParamValues[2] = null;
                        ifFunc.Info.Set(voidFuncIfTrue.Info);
                    }

                    ifFunc.ParamValues[1] = null;
                    ifFunc.Next = null;
                    ifFunc.Info.Set(generatorFunc.Info);
                    ifFunc.Info.IfCount++;
                    ifFunc.Info.IfDepth = ifFunc.Parent.Info.IfDepth + 1;
                    ifFunc.Info.States = generatorFunc.Info.States;
                    ifFunc.Info.IfExecCount = generatorFunc.Info.IfExecCount + ifFunc.Parent.Info.Scope.Count;
                    ifFunc.Info.Scope = inScope; //??
                }

                ifFunc.ParamValues[0] = null;
                ifFunc.ParamValues[1] = null;
                ifFunc.Info.Set(generatorFunc.Info);
                ifFunc.Info.IfCount++;
                ifFunc.Info.IfDepth = ifFunc.Parent.Info.IfDepth + 1;
                ifFunc.Info.IfExecCount = generatorFunc.Info.IfExecCount + ifFunc.Parent.Info.Scope.Count;
                ifFunc.Next = null;
            }
        }
        public bool CanHaveNext => true;
    }

    // If(boolExpr, whenTrueExpr, whenFalseExpr)
    public class IfElseFunction : BaseFunction
    {
        public override string Name => "ifElse";

        public IfElseFunction(BaseFunction parent)
        {
            ParamValues = new BaseFunction[3];
            this.Parent = parent;
        }

        public override bool IsValid()
        {
            return true;
        }
        
        public override object Eval()
        {
            ExecutionCounter.Evaluated?.Invoke(this);

            if ((bool)ParamValues[0].Eval())
            {
                ParamValues[1]?.Eval();
            }
            else
            {
                ParamValues[2]?.Eval();
            }

            Next?.Eval();

            return null;
        }

        public override BaseFunction Copy(BaseFunction parent)
        {
            var copy = new IfFunction( parent );
            copy.ParamValues = new[] { ParamValues[0].Copy(copy), ParamValues[1].Copy(copy), ParamValues[2].Copy(copy) };
            copy.Next = Next?.Copy(copy);
            copy.Info.Set(this.Info);
            return copy;
        }

        public override string ToString()
        {
            return $"if({ParamValues[0]})" + "{ " + $"{ParamValues[1]}" + " } else {" + ParamValues[2] + " }" + (Next == null ? "" : " next:" + Next);
        }
    }
}
