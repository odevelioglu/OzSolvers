using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySolver.Tests
{
    [TestClass]
    public class StateHolderTest
    {
        [TestMethod]
        public void StateHolder_test()
        {
            var holder = new StateHolder();

            //holder.Init(3);

            //holder.StateCollections[0].FillSwapResults();

            //var swap = holder.StateCollections[0].SwapResults.First();
            //holder.ApplySwap(swap.A, swap.B);

            holder.Init(4);
            holder.StateCollections[0].FillSwapResults();
                        
            holder.ApplySwap(0, 1);
            holder.StateCollections[1].FillSwapResults();

            holder.ApplySwap(1, 2);
            holder.StateCollections[2].FillSwapResults();

            holder.ApplyRotate(1, 3, 2);
        }
    }

    public class StateHolder
    {
        public List<StateCollection> StateCollections { get; set; }

        public void Init(int lenght)
        {
            var states = Enumerable.Range(1, lenght).Permute();

            StateCollections = new List<StateCollection>
            {
                new StateCollection(states.Select(p => new State(p)))
            };
        }

        public void ApplySwap(int i, int j)
        {
            var swapped = StateCollections.Last().Swap(i, j);
            StateCollections.Add(swapped);
        }

        public void ApplyRotate(int i, int j, int size)
        {
            var swapped = StateCollections.Last().Rotate(i, j, size);
            StateCollections.Add(swapped);
        }
    }

    public class StateCollection
    {
        public List<State> InnerList = new List<State>();

        public StateCollection()
        {
        
        }

        public StateCollection(IEnumerable<State> list) 
        {
            InnerList.AddRange(list);
        }

        public List<SwapResult> SwapResults = new List<SwapResult>();

        public void FillSwapResults()
        {
            foreach(var swap in SwapResult.GetAllPossibleSwaps(InnerList.First().InnerList.Count))
            {
                var swapped = Swap(swap.A, swap.B);
                swap.SortedCount = swapped.SortedCount;

                SwapResults.Add(swap);
            }
        }

        public int SortedCount => InnerList.Count(p => p.IsSorted);

        public StateCollection Swap(int i, int j)
        {
            var ret = new StateCollection();
            var copy = this.Copy();
            
            foreach(var list in copy.InnerList)
            {
                ret.InnerList.Add(list.Swap(i, j));
            }

            return ret;
        }

        public StateCollection Rotate(int i, int j, int size)
        {
            var ret = new StateCollection();
            var copy = this.Copy();

            foreach (var list in copy.InnerList)
            {
                ret.InnerList.Add(list.Rotate(i, j, size));
            }

            return ret;
        }

        public StateCollection Copy()
        {
            return new StateCollection(this.InnerList.Select(p=>p.Copy()));
        }

        public override string ToString()
        {
            return string.Join("-", InnerList.Select(p=>p.ToString()));
        }
    }

    public class State
    {
        public List<int> InnerList = new List<int>();

        public State(IEnumerable<int> list)
        {
            InnerList.AddRange(list);
        }

        public bool IsSorted
        {
            get
            {
                for(int i = 0; i < InnerList.Count-1; i++)
                {
                    if (InnerList[i] > InnerList[i + 1])
                        return false;
                }

                return true;
            }
        }
        
        public State Copy()
        {
            return new State(this.InnerList.ToList());
        }

        public State Swap(int i, int j)
        {
            var copy = InnerList.ToList();

            copy.IfSwap(i, j);

            return new State(copy);
        }

        public State Rotate(int i, int j, int size)
        {
            var copy = InnerList.ToList();
            copy.Rotate(i, j, size);
            return new State(copy);
        }

        public override string ToString()
        {
            return string.Join("", InnerList);
        }
    }

    public class SwapResult
    {
        public int A;
        public int B;

        public int SortedCount;

        public SwapResult(int i, int j)
        {
            A = i;
            B = j;
        }

        public static IEnumerable<SwapResult> GetAllPossibleSwaps(int len)
        {
            for (var i = 0; i < len - 1; i++)
            {
                for (var m = i + 1; m < len; m++)
                {
                    yield return new SwapResult(i, m);
                }
            }
        }
    }

    public static class Tools
    {
        public static void IfSwap(this List<int> list, int i, int j)
        {
            if (list[i] < list[j])
                return;

            var tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }

        public static void Rotate(this List<int> list,  int i, int j, int size)
        {
            if (list[i] < list[j])
                return;

            for (int n = 0; n < size; n++)
            {
                RotateOne(list);
            }
        }

        public static List<int> RotateOne(this List<int> list)
        {
            var copy = list.ToList();

            var tmp = copy.First();
            copy.RemoveAt(0);
            copy.Add(tmp);

            return copy;
        }
    }

}
