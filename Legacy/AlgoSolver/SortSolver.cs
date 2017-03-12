using System.Collections.Generic;
using System.Linq;

namespace OzAlgo
{
    public class SortSolver : AlgoSolver
    {
        private List<int> List;

        public SortSolver(List<int> list)
        {
            List = list;
        }

        public override List<IFuncBase> GetFunctions()
        {
            return new List<IFuncBase> { new FuncSwapIfGt(List.Count)};
        }

        public override bool Test(FunNode node)
        {          
            var list = new List<int>(List);

            // Run the program
            //foreach (var args in FunTree.VisitToTop(node))
            //{
            //    var func = new FuncSwapIfGt(copy);
            //    func.Call(copy, args);
            //}

            // Run program
            var funNodes = FunTree.GetNodes(node);
            foreach (var nod in funNodes)
            {
                nod.Function.Call(list, nod.Args);
            }

            // Test results
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i] > list[i + 1])
                    return false;
            }
            
            return true;
        }
    }

    public class FuncSwapIfGt : FuncBase, IFuncBase
    {
        private int _count;
        
        public FuncSwapIfGt(int count)
        {
            this._count = count;
        }

        public void Call(List<int> list, List<int> args)
        {
            var x = (int)args[0];

            if (list[x] > list[x + 1])
            {
                var tmp = list[x + 1];
                list[x + 1] = list[x];
                list[x] = tmp;
            }
        }
        
        public IEnumerable<List<int>> GetPossibleArguments()
        {
            return Enumerable.Range(0, _count - 1).Select(p => new List<int>() { p });
        }

        public bool CanAdd(FunNode child, FunNode parent)
        {
            if (Count(parent) + 1 > _count * (_count - 1) / 2) return false;

            // Can you find these reducibles automatically ??
            // Implement InvariantSolver: finds invariants of a function => call funct with different values, check results

            // repeating same as parent
            if (!parent.IsRoot && child.Args.SequenceEqual(parent.Args))
                return false;

            // You can eliminate the case of f(1); f(0); f(1); f(0); f(1); f(0);
            // On the same 2 elements (0 and 1) you can't call f more than 3 times consequtively
            // So the 2nd f(0); sould be eliminated

            //child.ParentNode = parent;
            //if (IsReducible(FunTree.VisitToTop(child)))
            //    return false;

            // Eliminate symetric solutions
            //child.ParentNode = parent;
            //var childList = FunTree.VisitToTop(child);
            //childList.Reverse();

            //var root = FunTree.GetRoot(parent);
            //foreach (var leaf in FunTree.GetLeafList(root))
            //{
            //    if (FunTree.VisitToTop(leaf).SequenceEqual(childList))
            //        return false;
            //}

            return true;
        }

        public string FuncName
        {
            get
            {
                return "SwapIfGt";
            }
        }

        // checks if a swap series comes back to a previous state 
        //public bool IsReducible(List<List<int>> swapSeries)
        //{
        //    var state = List.ToList();
        //    var set = new HashSet<List<int>>(new OrderedListEqualityComparer()) { state.ToList() };

        //    foreach (var s in swapSeries)
        //    {
        //        SwapIfGt(state, s[0]);

        //        if (set.Contains(state))
        //            return true;

        //        set.Add(state.ToList());
        //    }

        //    return false;
        //}
    }

    
}
