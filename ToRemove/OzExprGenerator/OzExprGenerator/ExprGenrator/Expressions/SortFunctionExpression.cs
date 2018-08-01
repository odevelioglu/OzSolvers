using System;
using System.Collections.Generic;
using System.Linq;

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
            
            foreach (var voidFunc in context.VoidExpressions.OfType<IfFunctionExpression>().SelectMany(p=>p.GenerateDepthFirst(context, sort))) // TODO: sort specific
            {
                sort.ParamValues[1] = voidFunc;
                yield return sort;
                sort.Info = new FuncInfo();
                sort.ParamValues[1] = null;
            }

            yield return sort;
        }

        public IEnumerable<BaseFunction> GenerateStateFull(Context context, BaseFunction parentFunc, bool isNext)
        {
            var sortFunc = new SortFunction { Parent = parentFunc };
            
            var list = ((ListConstant)context.List).Value;
            var states = new List<int[]>();
            foreach (var perm in list.Permute())
            {
                states.Add(perm.ToArray());
            }
            sortFunc.Info.States = states;
            //sortFunc.Info.StateHistory = new List<int[]>[states.Count];
            //for (var i = 0; i < states.Count; i++) sortFunc.Info.StateHistory[i] = new List<int[]>();
            //sortFunc.Info.HistorizeState(states);

            sortFunc.Info.Scope = Enumerable.Range(0, sortFunc.Info.States.Count).ToList();
            
            sortFunc.ParamValues[0] = context.List;

            yield return sortFunc;

            foreach (var voidFunc in context.VoidExpressions.SelectMany(p => p.GenerateStateFull(context, sortFunc, false))) // TODO: sort specific
            {
                sortFunc.ParamValues[1] = voidFunc;
                sortFunc.Info.Set(voidFunc.Info);
                sortFunc.Info.States = voidFunc.Info.States;
                //sortFunc.Info.StateHistory = voidFunc.Info.StateHistory;
                yield return sortFunc;
                sortFunc.ParamValues[1] = null;
                sortFunc.Info.IfCount = 0;
                sortFunc.Info.SwapCount = 0;
                sortFunc.Info.IfDepth = 0;
                sortFunc.Info.IfExecCount = 0;
                sortFunc.Info.States = states;
                //sortFunc.Info.StateHistory = new List<int[]>[states.Count];
                //for (var i = 0; i < states.Count; i++) sortFunc.Info.StateHistory[i] = new List<int[]>();
                //sortFunc.Info.HistorizeState(states);
            }
        }

        public bool CanHaveNext => false;
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

            ParamValues[1]?.Eval();
            return ParamValues[0].Eval();
        }

        public override string ToString()
        {
            return "sort(A)" + "{" + ParamValues[1] + "}";
        }

        public override BaseFunction Copy(BaseFunction parent)
        {
            var copy = new SortFunction { Parent = parent };
            copy.ParamValues = new[] { ParamValues[0].Copy(copy), ParamValues[1]?.Copy(copy) };
            copy.Info.Set(this.Info);
            return copy;
        }
    }
}
