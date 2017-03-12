using System.Collections.Generic;

namespace OzAlgo
{
    public class AlgoSolver
    {
        public FunNode Root = new FunNode();
        public List<FunNode> ResultNodes = new List<FunNode>();
        
        public virtual bool Test(FunNode node)
        {
            return true;
        }

        public virtual List<IFuncBase> GetFunctions()
        {
            return new List<IFuncBase>();
        }

        public void Expand(FunNode node)
        {
            foreach (var funcBase in GetFunctions())
            {
                foreach (var pos in funcBase.GetPossibleArguments())
                {
                    var child = new FunNode()
                    {
                        Args = pos, 
                        Function = funcBase
                    };

                    if (!funcBase.CanAdd(child, node)) continue;

                    node.AddChild(child);
                
                    Expand(child);
                    
                    if(Test(child))
                        ResultNodes.Add(child);
                }
            }

            if (node.Function is FuncBaseIfLt)
            {
                foreach (var funcBase in GetFunctions())
                {
                    foreach (var pos in funcBase.GetPossibleArguments())
                    {
                        var callIf = new FunNode()
                        {
                            Args = pos,
                            Function = funcBase,
                            IsSubCallRoot = true
                        };

                        if (!funcBase.CanAdd(callIf, node)) continue;

                        node.AddSubCall(callIf);

                        Expand(callIf);

                        if (Test(callIf))
                            ResultNodes.Add(callIf);
                    }
                }
            }
        }

        public void Solve()
        {
            if (Test(Root))
                ResultNodes.Add(Root);

            // Continue on the previous solution
            Expand(Root);
        }
    }
}
