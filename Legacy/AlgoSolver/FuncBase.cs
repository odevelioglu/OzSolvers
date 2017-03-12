using System;
using System.Collections.Generic;

namespace OzAlgo
{
    public interface IFuncBase
    {
        void Call(List<int> list, List<int> args);
        IEnumerable<List<int>> GetPossibleArguments();
        bool CanAdd(FunNode child, FunNode parent);

        string FuncName { get; }
    }

    public class FuncBase 
    {
        public int Count(FunNode parent)
        {
            int count = 0;
            FunTree.VisitToTop(parent, funNode =>
            {
                if (funNode.Function != null &&
                    funNode.Function.GetType() == this.GetType())
                    count++;
            });

            return count;
        }
    }
}