using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExprGenrator
{
    public interface IExpression
    {
        List<Type> ParameterTypes { get; }
        Type ReturnType { get; }
        IEnumerable<BaseFunction> GenerateDepthFirst(Context context, BaseFunction parentFunc);
        IEnumerable<BaseFunction> GenerateStateFull(Context context, BaseFunction parentFunc, bool isNext);

        bool CanHaveNext { get; }
    }

    //https://en.wikipedia.org/wiki/Backtracking
    //https://en.wikipedia.org/wiki/Local_consistency
    public class FuncInfo
    {
        public int IfCount;
        public int IfDepth;
        public int SwapCount;
        public int IfExecCount;

        //public List<int[]>[] StateHistory;
        public List<int[]> States = new List<int[]>();
        public List<int> Scope = new List<int>();

        //public void HistorizeState(List<int[]> states)
        //{
        //    for (int i = 0; i < states.Count; i++)
        //    {
        //        this.StateHistory[i].Add(states[i]);
        //    }
        //}

        public void Set(FuncInfo info)
        {
            this.IfCount = info.IfCount;
            this.SwapCount = info.SwapCount;
            this.IfDepth = info.IfDepth;
            this.IfExecCount = info.IfExecCount;
        }

        public void SetStates(FuncInfo info)
        {
            this.States = info.States;
            this.Scope = info.Scope; 
        }

        public IEnumerable<int[]> ScopedStates
        {
            get
            {
                foreach (var orderNumber in this.Scope)
                {
                    yield return this.States[orderNumber];
                }
            }
        }

        //public string HistoryString()
        //{
        //    var builder = new StringBuilder();
        //    builder.AppendLine("history:");
        //    for (var historyIndex = 0; historyIndex < this.StateHistory.Count(); historyIndex++)
        //    {
        //        var history = string.Join("; ", StateHistory[historyIndex].Select(p => string.Join(",", p)));
        //        builder.AppendLine(history);
        //    }

        //    return builder.ToString();
        //}

        public override string ToString()
        {
            return $"swap:{SwapCount} if:{IfCount} States:{string.Join(", ", this.States.Select(p=>string.Join("", p)))} Scope:{string.Join(",", this.Scope)}";
        }
    }

    public class BaseFunction
    {
        public virtual string Name { get; }
        public BaseFunction Parent { get; set; }

        public BaseFunction[] ParamValues { get; set; }
        public BaseFunction Next { get; set; }
        public BaseFunction Before { get; set; }

        public FuncInfo Info = new FuncInfo();

        public virtual bool IsValid()
        {
            throw new NotImplementedException();
        }

        public virtual object Eval()
        {
            throw new NotImplementedException();
        }

        public virtual BaseFunction Copy(BaseFunction parent)
        {
            throw new NotImplementedException();
        }
    }

}
