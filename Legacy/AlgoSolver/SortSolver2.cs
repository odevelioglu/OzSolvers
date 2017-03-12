using System.Collections.Generic;
using System.Linq;
using Facet.Combinatorics;

namespace OzAlgo
{
    public class SortSolver2 : AlgoSolver
    {
        private List<int> List;

        public SortSolver2(List<int> list)
        {
            List = list;
        }

        public override List<IFuncBase> GetFunctions()
        {
            return new List<IFuncBase> { new FuncSwap(List.Count) };
        }

        public override bool Test(FunNode node)
        {
            var list = new List<int>(List);

            // Run program
            foreach (var nod in FunTree.GetNodes(node))
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
    
    public class FuncSwap : FuncBase, IFuncBase
    {
        private int _count;

        public string FuncName
        {
            get
            {
                return "Swap";
            }
        }

        public FuncSwap(int count)
        {
            this._count = count;
        }

        public void Call(List<int> list, List<int> args)
        {
            var x = args[0];
            var y = args[1];

            if (list[x] > list[y])
            {
                var tmp = list[y];
                list[y] = list[x];
                list[x] = tmp;
            }
        }

        public IEnumerable<List<int>> GetPossibleArguments()
        {
            if (_count < 2)
                return new List<int>[0];

            var list = Enumerable.Range(0, _count).ToList();

            var combinations = new Combinations<int>(list, 2, GenerateOption.WithoutRepetition);

            return combinations.Cast<List<int>>();
        }

        public bool CanAdd(FunNode child, FunNode parent)
        {
            if (Count(parent) + 1 > _count - 1) return false;

            // repeating same as parent
            if (!parent.IsRoot && child.Args.HasSameElements(parent.Args))
                return false;

            //if (IsReducible(child, parent))
            //    return false;

            //if (!CheckComplexityPassed(child, parent)) 
            //   return false;

            return true;
        }

        // You can eliminate the case of f(1); f(0); f(1); f(0); f(1); f(0);
        // On the same 2 elements (0 and 1) you can't call f more than 3 times consequtively
        // So the 2nd f(0); sould be eliminated
        private static bool CheckComplexityPassed(FunNode child, FunNode parent)
        {
            child.ParentNode = parent;
            var list = FunTree.VisitToTop(child);
            list.Reverse();

            var last4 = list.Take(4);

            if (last4.Count() == 4)
            {
                var set = new HashSet<int>();
                foreach (var args in list)
                {
                    if (!set.Contains(args[0]))
                        set.Add(args[0]);

                    if (!set.Contains(args[1]))
                        set.Add(args[1]);
                }

                if (set.Count == 3)
                {
                    child.ParentNode = null;
                    return false;
                }
            }

            child.ParentNode = null;
            return true;
        }

        // checks if a swap series comes back to a previous state 
        //public bool IsReducible(FunNode child, FunNode parent)
        //{
        //    child.ParentNode = parent;
        //    var swapSeries = FunTree.VisitToTop(child);

        //    var state = _list.ToList();
        //    var set = new HashSet<List<int>>(new OrderedListEqualityComparer()) { state.ToList() };

        //    foreach (var s in swapSeries)
        //    {
        //        Call(state, s);

        //        if (set.Contains(state))
        //        {
        //            child.ParentNode = null;
        //            return true;
        //        }

        //        set.Add(state.ToList());
        //    }

        //    child.ParentNode = null;
        //    return false;
        //}
    }
}
