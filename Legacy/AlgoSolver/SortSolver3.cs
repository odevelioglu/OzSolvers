using System;
using System.Collections.Generic;
using System.Linq;
using Facet.Combinatorics;

namespace OzAlgo
{
    public class SortSolver3 : AlgoSolver
    {
        private List<int> List;

        public SortSolver3(List<int> list)
        {
            List = list;
        }

        public override List<IFuncBase> GetFunctions()
        {
            return new List<IFuncBase> { new FuncBaseIfLt(List.Count) };
        }

        public override bool Test(FunNode node)
        {
            //var list = new List<int>(List);

            //// Run program
            //foreach (var nod in FunTree.GetNodes(node))
            //{
            //    nod.Function.Call(list, nod.Args);
            //}

            //// Test results
            //for (int i = 0; i < list.Count - 1; i++)
            //{
            //    if (list[i] > list[i + 1])
            //        return false;
            //}

            return true;
        }
    }

    public class FuncBaseIfLt : FuncBase, IFuncBase
    {
        public FunNode SubCall = null;

        private int _count;
        public string FuncName
        {
            get
            {
                return "if";
            }
        }

        public FuncBaseIfLt(int count)
        {
            this._count = count;
        }

        public void Call(List<int> list, List<int> args)
        {
            //var x = (int)args[0];
            //var y = (int)args[1];

            //if (_list[x] < _list[y])
            //{
            //    SubCall.Function.Call();
            //}
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
            if (child.Function is FuncBaseIfLt)
            { 
                if (child.Args[0] >= child.Args[1])
                    return false;
            
                //if (Count(parent) > 3) return false;

                // repeating same as parent
                //if (!parent.IsRoot && child.Args.SequenceEqual(parent.Args))
                //    return false;
            
                child.ParentNode = parent;
                if (HasRepeting(child))
                    return false;
            }

            return true;
        }

        private HashSet<List<int>> _cache = new HashSet<List<int>>(new OrderedListEqualityComparer());
        private bool HasRepeting(FunNode child)
        {
            _cache.Clear();

            var repeated = false;
            FunTree.VisitToTop(child, node =>
            {
                if (node.Function is FuncBaseIfLt)
                { 
                    if (_cache.Contains(node.Args))
                    {
                        repeated = true;
                        return;
                    }

                    _cache.Add(node.Args);
                }
            });

            return repeated;
        }
    }

    public class FuncSwap2 : FuncBase, IFuncBase
    {
        private int _count;

        public string FuncName
        {
            get
            {
                return "sw";
            }
        }

        public FuncSwap2(int count)
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
            if (!parent.IsRoot && 
                parent.Function is FuncSwap2 &&
                child.Args.HasSameElements(parent.Args))
                return false;
            
            return true;
        }
    }
}
