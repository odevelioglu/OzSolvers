using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace MySolver.Tests
{
    [TestFixture]
    public class StateHolderTest
    {
        [Test]
        public void StateHolder_testListOf3()
        {
            var holder = new StateHolder();

            holder.Init(3);

            holder.ApplySwap(0, 1);
            holder.ApplySwap(1, 2); //4 should go inner 

            holder.ApplySwap(0, 1); //normally inside

            Assert.IsTrue(holder.StateCollections.Last().SortedCount == 6);
        }

        [Test]
        public void StateHolder_testListOf4()
        {
            var holder = new StateHolder();
            
            holder.Init(4);
            holder.ApplySwap(0, 1);
            holder.ApplySwap(2, 3);            
            holder.ApplyRotate(1, 3, 2);
            holder.ApplySwap(1, 2);
            Assert.IsTrue(holder.StateCollections.Last().ShouldGoInner);

            holder.ApplySwap(0, 1); // in inner scope

            Assert.IsTrue(holder.StateCollections.Last().SortedCount == 24);
        }

        [Test]
        public void StateHolder_testListOf4_withMoveRight()
        {
            var holder = new StateHolder();

            holder.Init(4);
            holder.ApplyMoveRight(0, 1, new[] { 1, 3, 0, 0 });
            holder.ApplyMoveRight(2, 3, new[] { 0, 0, 1, 3 });
            holder.ApplyMoveRight(1, 3, new[] { 2, 2, 2, 2 });
            holder.ApplyMoveRight(1, 2, new[] { 0, 1, 3, 0 });
            Assert.IsTrue(holder.StateCollections.Last().ShouldGoInner);

            holder.ApplyMoveRight(0, 1, new[] { 1, 3, 0, 0 }); // in inner scope

            Assert.IsTrue(holder.StateCollections.Last().SortedCount == 24);
        }

        [Test]
        public void StateHolder_testListOf4BubleSort()
        {
            // Restriction "double the correct results" leads to quicker sort
            // Restriction "improuve the correct results" leads to both bubleSort and quicker sort
            var holder = new StateHolder();

            holder.Init(4);
            holder.ApplySwap(0, 1); //2
            holder.ApplySwap(1, 2); //4
            holder.ApplySwap(2, 3); //8
            holder.ApplySwap(0, 1); //12
            holder.ApplySwap(1, 2); //18 => ShouldGoInner
            holder.ApplySwap(0, 1); //24 

            Assert.IsTrue(holder.StateCollections.Last().SortedCount == 24);
        }

        [Test]
        public void StateHolder_testListOf5()
        {
            var holder = new StateHolder();

            holder.Init(5);
            holder.ApplyMoveRight(0, 1, new[] { 1, 4, 0, 0, 0 });
            holder.ApplyMoveRight(2, 3, new[] { 0, 0, 1, 4, 0 });
                        
            foreach(var move in holder.StateCollections.Last().StateChangeResults.Where(p => p.SortedCount == 8))
            {
                holder.ApplyMoveRight(move.A, move.B, move.Moves);
                if (holder.StateCollections.Last().ShouldGoInner) throw new Exception("sd");

                foreach (var move16 in holder.StateCollections.Last().StateChangeResults.Where(p => p.SortedCount == 16))
                {
                    holder.ApplyMoveRight(move16.A, move16.B, move16.Moves);
                    if (holder.StateCollections.Last().ShouldGoInner) throw new Exception("sd");

                    //var moves32 = holder.StateCollections.Last().StateChangeResults.Where(p => p.SortedCount == 32);
                    //if(moves32.Any() && holder.StateCollections.Last().ShouldGoInner)
                    //{

                    //}

                    foreach (var move32 in holder.StateCollections.Last().StateChangeResults.Where(p => p.SortedCount == 32))
                    {
                        holder.ApplyMoveRight(move32.A, move32.B, move32.Moves);
                        if (holder.StateCollections.Last().ShouldGoInner) throw new Exception("sd");

                        foreach (var move64 in holder.StateCollections.Last().StateChangeResults.Where(p => p.SortedCount == 64))
                        {
                            holder.ApplyMoveRight(move64.A, move64.B, move64.Moves);
                            if (holder.StateCollections.Last().ShouldGoInner) throw new Exception("sd");

                            var move120 = holder.StateCollections.Last().StateChangeResults.Where(p => p.SortedCount == 120);
                            if(move120.Any())
                            {

                            }

                            holder.StateCollections.RemoveAt(holder.StateCollections.Count - 1);
                        }

                        holder.StateCollections.RemoveAt(holder.StateCollections.Count - 1);
                    }

                    holder.StateCollections.RemoveAt(holder.StateCollections.Count - 1);
                }

                holder.StateCollections.RemoveAt(holder.StateCollections.Count - 1);
            }
        }

        [Test]
        public void StateHolder_testTMP()
        {
            var lists = new List<int> { 5, 4, 3, 2, 1 }.Permute().Select(p=>p.ToList()).ToList();

            var counts = new List<int>();

            foreach(var list in lists)
            {
                //count = 0;
                Sort5(list);
                //counts.Add(count);
                //Assert.IsTrue(IsSorted(list));
            }

            var count = lists.Count(IsSorted);

            var tmp = counts.GroupBy(p => p).Select(x => new { x.Key, count=x.Count() }).ToList();
        }

        private int count;

        private void Sort5(List<int> list)
        {
            if (list[0] > list[1]) //2
            {
                Swap(list, 0, 1);
            }

            if (list[2] > list[3]) //4
            {
                Swap(list, 2, 3);
            }

            if (list[3] > list[4]) //8
            {
                Swap(list, 3, 4);
                if (list[2] > list[3]) //12
                {                    
                    Swap(list, 2, 3);
                }
            }

            //Merge
        }

        private void Sort4(List<int> list)
        {
            if(list[0] > list[1]) //12
            {                
                Swap(list, 0, 1);
            }

            if (list[2] > list[3]) //12
            {
                
                Swap(list, 2, 3);
            }

            if (list[1] > list[3]) //12
            {
                
                Swap(list, 1, 3);
                Swap(list, 0, 2);
            }

            if (list[1] > list[2]) //16
            {
                
                Swap(list, 1, 2);
                if (list[0] > list[1]) //8
                {
                    count++;
                    Swap(list, 0, 1);
                }
            }
        }

        private void Sort4_2(List<int> list)
        {
            if (list[0] > list[1]) //12
            {
                count++;
                Swap(list, 0, 1);
            }

            if (list[2] > list[3]) //12
            {
                count++;
                Swap(list, 2, 3);
            }

            if (list[0] > list[2]) //12
            {
                count++;
                Swap(list, 0, 2);
                count++;
                Swap(list, 1, 3);
            }

            if (list[1] > list[2]) //16
            {
                count++;
                Swap(list, 1, 2);
                if (list[2] > list[3]) //8
                {
                    count++;
                    Swap(list, 2, 3);
                }
            }
        }

        public void Swap(List<int> list, int i, int j)
        {
            var tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }

        public bool IsSorted(List<int> list)
        {            
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i] > list[i + 1])
                    return false;
            }

            return true;            
        }

        private IEnumerable<StateChangeResult> Push(StateHolder holder, List<StateChangeResult> tmp)
        {
            foreach (var chnage in tmp)
            {
                if (chnage.Name == "swap")
                    holder.ApplySwap(chnage.A, chnage.B);
                else
                    holder.ApplyRotate(chnage.A, chnage.B, chnage.C);

                var count2 = Math.Pow(2, holder.StateCollections.Count);
                var tmp2 = holder.StateCollections.Last().StateChangeResults.Where(p => p.SortedCount == count2).ToList();

                foreach(var c in tmp2)
                {
                    yield return c;
                }

                holder.StateCollections.RemoveAt(holder.StateCollections.Count - 1);
            }
        }

        [Test]
        public void StateHolder_testListOf6()
        {
            var holder = new StateHolder();

            holder.Init(6);
            holder.ApplySwap(0, 1); //2
            holder.ApplySwap(2, 3); //4
            holder.ApplySwap(4, 5); //8     
            holder.ApplyRotate(2, 5, 4); //16   
            holder.ApplySwap(1, 2);
            holder.ApplySwap(3, 4);

            var count = Math.Pow(2, holder.StateCollections.Count);
            var tmp = holder.StateCollections.Last().StateChangeResults.Where(p => p.SortedCount == count).ToList();

            //pos
            var pos = Push(holder, tmp).ToList();

        }

        [Test]
        public void StateHolder_RotateTest()
        {
            var state = new State(new[] { 4, 3, 2, 1 });             
            var rotated = state.Rotate(1, 3, 2);

            Assert.IsTrue(rotated.InnerList.SequenceEqual(new[] { 2, 1, 4, 3 }));

            var rotates = Generator.GenerateRotate(4);
            Assert.IsTrue(rotates.Count() == 3);
        }
    }


    public class StateHolder
    {
        public List<StateCollection> StateCollections { get; set; }

        public void Init(int lenght)
        {
            var states = Enumerable.Range(1, lenght).Permute();

            var col = new StateCollection(states.Select(p => new State(p) { IsInScope = true }));
            col.FillStateChangeResults();
            StateCollections = new List<StateCollection> { col };
        }

        public void ApplySwap(int i, int j)
        {
            var swapped = StateCollections.Last().Swap(i, j);

            swapped.FillStateChangeResults();

            StateCollections.Add(swapped);
        }

        public void ApplyRotate(int i, int j, int count)
        {
            var rotated = StateCollections.Last().Rotate(i, j, count);

            rotated.FillStateChangeResults();

            StateCollections.Add(rotated);
        }

        public void ApplyMoveRight(int i, int j, int[] moves)
        {
            var rotated = StateCollections.Last().MoveRight(i, j, moves);

            rotated.FillStateChangeResults();

            StateCollections.Add(rotated);
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

        public List<StateChangeResult> StateChangeResults = new List<StateChangeResult>();
        public bool ShouldGoInner;

        public void FillStateChangeResults()
        {
            foreach(var change in Generator.GenerateAll(InnerList.First().InnerList.Count))
            {
                if(change.Name == "swap")
                { 
                    var swapped = Swap(change.A, change.B);
                    change.SortedCount = swapped.SortedCount;

                    StateChangeResults.Add(change);
                }

                if (change.Name == "rotate")
                {
                    var rotated = Rotate(change.A, change.B, change.C);
                    change.SortedCount = rotated.SortedCount;

                    StateChangeResults.Add(change);
                }

                if (change.Name == "moveRight")
                {
                    var rotated = MoveRight(change.A, change.B, change.Moves);
                    change.SortedCount = rotated.SortedCount;

                    StateChangeResults.Add(change);
                }
            }

            // Go inner if outer scope is all sorted
            if( InnerList.Where(s => !s.IsInScope).Any() &&
                InnerList.Where(s => !s.IsInScope).All(s => s.IsSorted))
                ShouldGoInner = true;
        }

        public int SortedCount => InnerList.Count(p => p.IsSorted);

        public StateCollection Swap(int i, int j)
        {
            var ret = new StateCollection();
            var copy = this.Copy();
            
            foreach(var state in copy.InnerList)
            {
                ret.InnerList.Add(state.Swap(i, j));
            }

            return ret;
        }

        public StateCollection Rotate(int i, int j, int count)
        {
            var ret = new StateCollection();
            var copy = this.Copy();

            foreach (var list in copy.InnerList)
            {
                ret.InnerList.Add(list.Rotate(i, j, count));
            }

            return ret;
        }

        public StateCollection MoveRight(int i, int j, int[] moves)
        {
            var ret = new StateCollection();
            var copy = this.Copy();

            foreach (var state in copy.InnerList)
            {
                ret.InnerList.Add(state.MoveRight(i, j, moves));
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
        public bool IsInScope;

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

            var isInScope = false;
            if (copy[i] > copy[j])
                isInScope = true ;

            copy.IfSwap(i, j);                      

            return new State(copy) { IsInScope = isInScope };
        }

        public State Rotate(int i, int j, int count)
        {
            var copy = InnerList.ToList();

            var isInScope = false;
            if (copy[i] > copy[j])
                isInScope = true;

            copy.Rotate(i, j, count);

            return new State(copy) { IsInScope = isInScope };
        }

        public State MoveRight(int i, int j, int[] moves)
        {
            //var isInScope = (InnerList[i] < InnerList[j]);
            
            if (InnerList[i] < InnerList[j])
            { 
                var cop = this.Copy();
                cop.IsInScope = false;
                return cop;
            }

            var copy = InnerList.MoveRight(moves);

            return new State(copy) { IsInScope = true }; // ?????
        }

        public override string ToString()
        {
            return (IsInScope ? "s" : " ") + string.Join("", InnerList) + (IsSorted ? "c": " ");
        }
    }

    public class StateChangeResult
    {
        public string Name;
        public int A;
        public int B;
        public int C;
        public int[] Moves;

        public int SortedCount;

        public StateChangeResult(string name, int i, int j)
        {
            Name = name;
            A = i;
            B = j;
        }

        public StateChangeResult(string name, int i, int j, int c)
        {
            Name = name;
            A = i;
            B = j;
            C = c;
        }

        public StateChangeResult(string name, int i, int j, int[] moves)
        {
            Name = name;
            A = i;
            B = j;
            Moves = moves;            
        }

        public override string ToString()
        {
            if(Name == "swap")
                return "swap(" + A + "," + B + ")" + " :" + SortedCount;

            if (Name == "rotate")
                return "rotate(" + A + "," + B + "," + C +  ")" + " :" + SortedCount;

            return "moveRight(" + A + "," + B + ",{" + string.Join(",", Moves) + "})" + " :" + SortedCount;
        }
    }

    public class Generator
    {
        public static IEnumerable<StateChangeResult> GenerateSwap(int len)
        {
            for (var i = 0; i < len - 1; i++)
            {
                for (var m = i + 1; m < len; m++)
                {
                    yield return new StateChangeResult("swap", i, m);
                }
            }
        }

        public static IEnumerable<StateChangeResult> GenerateRotate(int len)
        {
            for (var i = 0; i < len - 2; i++)
            {
                for (var m = 2 + i; m < len; m++)
                {
                    for (var k = 1; k < len; k++)
                    {
                        yield return new StateChangeResult("rotate", i, m, k);
                    }
                }
            }
        }

        public static List<StateChangeResult> Cache;

        public static IEnumerable<StateChangeResult> GenerateMoveRight(int len)
        {
            if(Cache == null)
            {
                Cache = new List<StateChangeResult>();
                for (var i = 0; i < len - 1; i++)
                {
                    for (var m = i + 1; m < len; m++)
                    {
                        foreach(var par in MoveRightParamGenerator.GetParams(len))
                        {
                            Cache.Add( new StateChangeResult("moveRight", i, m, par) );
                        }                    
                    }
                }
            }

            foreach(var s in Cache)
            {
                yield return new StateChangeResult(s.Name, s.A, s.B, s.Moves);
            }
        }

        public static IEnumerable<StateChangeResult> GenerateAll(int len)
        {
            //return GenerateSwap(len).Concat(GenerateRotate(len));
            
            return GenerateMoveRight(len);
        }
    }


    public static class Tools
    {
        public static void IfSwap(this List<int> list, int i, int j)
        {
            Contract.Assert(j > i);

            if (list[i] < list[j])
                return;

            var tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }

        public static void Rotate(this List<int> list, int i, int j, int count)
        {
            Contract.Assert(j > i);

            if (list[i] < list[j])
                return;

            for (int n = 0; n < count; n++)
            {
                RotateOne(list);
            }
        }

        public static void RotateOne(this List<int> list)
        {
            var tmp = list.First();
            list.RemoveAt(0);
            list.Add(tmp);            
        }
    }

}
