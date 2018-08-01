using System;
using System.Collections.Generic;
using System.Linq;

namespace ExprGenrator
{
    public class IfFunctionExpression : IExpression
    {
        public int Max { get; set; } = int.MaxValue;
        public int MaxDepth { get; set; } = int.MaxValue;
        public int MaxExec { get; set; } = int.MaxValue;
        public IfFunctionExpression(int max)
        {
            this.Max = max;
        }

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
            if (generatorFunc.Info.IfCount >= this.Max) yield break;
            
            var ifFunc = isNext ? new IfFunction(generatorFunc.Parent) { Before = generatorFunc } : new IfFunction(generatorFunc);
            
            // Get the scope from parent. Get the states from generator
            ifFunc.Info.Set(generatorFunc.Info);
            ifFunc.Info.IfCount++;
            ifFunc.Info.IfDepth = ifFunc.Parent.Info.IfDepth + 1;
            if (ifFunc.Info.IfDepth > this.MaxDepth) yield break;

            ifFunc.Info.IfExecCount = generatorFunc.Info.IfExecCount + ifFunc.Parent.Info.Scope.Count;
            if (ifFunc.Info.IfExecCount > this.MaxExec) yield break;

            foreach (var boolFunc in context.BoolExpressions.SelectMany(p => p.GenerateStateFull(context, ifFunc, false))) // does not change info
            {
                ifFunc.ParamValues[0] = boolFunc;

                // always get the states from the generator
                var par0 = ((IntConstant)boolFunc.ParamValues[0].ParamValues[1]).Value;
                var par1 = ((IntConstant)boolFunc.ParamValues[1].ParamValues[1]).Value;
                
                var scope = generatorFunc.Info.States.GetScopeSet(ifFunc.Parent.Info.Scope, p => p[par0] > p[par1], out var isChanged);
                if (scope.Count == 0 || !isChanged) // can be true and can be false
                {
                    ifFunc.ParamValues[0] = null;
                    ifFunc.Next = null;
                    continue;
                }
                
                ifFunc.Info.States = generatorFunc.Info.States;
                //ifFunc.Info.StateHistory = generatorFunc.Info.StateHistory;
                ifFunc.Info.Scope = scope;
                
                if (ifFunc.Info.Scope.Count >= ifFunc.Parent.Info.Scope.Count)
                {
                    throw new Exception("Wrong");
                }

                foreach (var voidFuncIfTrue in context.VoidExpressions.OfType<SwapFunctionExpression>().SelectMany(p => p.GenerateStateFull(context, ifFunc, false))) //first param
                {
                    ifFunc.ParamValues[1] = voidFuncIfTrue;
                    ifFunc.Info.Set(voidFuncIfTrue.Info); // voidFuncIfTrue contains alewady if
                    ifFunc.Info.States = voidFuncIfTrue.Info.States;
                    //ifFunc.Info.StateHistory = voidFuncIfTrue.Info.StateHistory;

                    yield return ifFunc;
                    
                    foreach (var nextVoidFunc in context.VoidExpressions.OfType<IfFunctionExpression>().SelectMany(p => p.GenerateStateFull(context, ifFunc, true))) // then NEXT
                    {
                        ifFunc.Next = nextVoidFunc;
                        ifFunc.Info.Set(nextVoidFunc.Info);
                        ifFunc.Info.States = nextVoidFunc.Info.States;
                        //ifFunc.Info.StateHistory = nextVoidFunc.Info.StateHistory;

                        yield return ifFunc;

                        ifFunc.Next = null;
                        ifFunc.Info.Set(voidFuncIfTrue.Info);
                        ifFunc.Info.States = voidFuncIfTrue.Info.States;
                        //ifFunc.Info.StateHistory = voidFuncIfTrue.Info.StateHistory;
                    }
                    
                    ifFunc.ParamValues[1] = null;
                    ifFunc.Next = null;
                    ifFunc.Info.Set(generatorFunc.Info);
                    ifFunc.Info.IfCount++;
                    ifFunc.Info.IfDepth = ifFunc.Parent.Info.IfDepth + 1;
                    ifFunc.Info.States = generatorFunc.Info.States;
                    ifFunc.Info.IfExecCount = generatorFunc.Info.IfExecCount + ifFunc.Parent.Info.Scope.Count;
                    //ifFunc.Info.StateHistory = generatorFunc.Info.StateHistory;
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

    public class IfFunction : BaseFunction
    {
        public override string Name => "if";

        public IfFunction(BaseFunction parent)
        {
            ParamValues = new BaseFunction[2];
            this.Parent = parent;
        }

        public override bool IsValid()
        {
            var depth = 0;
            var param = this.GetParams();

            var curr = this.ParamValues[1] as IfFunction;
            while (curr != null)
            {
                depth++;
                if (depth > 3) return false;

                if (this.HasParams(curr, param)) return false;

                curr = curr.ParamValues[1] as IfFunction;
            }

            return true;
        }

        private bool HasParams(IfFunction ifParent, int[] param)
        {
            var parentParams = ifParent.GetParams();

            //if(A[0] > A[1]){ if(A[0] > A[1]){ ...  } }
            if (parentParams[0] == param[0] && parentParams[1] == param[1])
                return true;
            
            return false;
        }

        public override object Eval()
        {
            ExecutionCounter.Evaluated?.Invoke(this);

            if ((bool)ParamValues[0].Eval())
            {
                ParamValues[1].Eval();
            }

            Next?.Eval();

            return null;
        }

        public override BaseFunction Copy(BaseFunction parent)
        {
            var copy = new IfFunction( parent );
            copy.ParamValues = new[] { ParamValues[0].Copy(copy), ParamValues[1].Copy(copy) };
            copy.Next = Next?.Copy(copy);
            copy.Info.Set(this.Info);
            return copy;
        }

        public override string ToString()
        {
            return $"if({ParamValues[0]})" + "{ " + $"{ParamValues[1]}" + " }" + (Next == null ? "" : " next:" + Next);
        }
    }
}
