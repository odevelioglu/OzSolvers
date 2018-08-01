using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MySolver.Tests
{
    [TestClass]
    public class SwapListSearcherTests
    {
        [TestMethod]
        public void Swap_GetAllPossibleSwaps_test()
        {
            var expected = new List<SwapFunc> { new SwapFunc(1, 2) };

            var searcher = new SwapListSearcher();
            var result = searcher.GetAllPossibleSwaps(new List<int> { 1, 3, 2, 4 });
            
            Assert.IsTrue(result.ToList().IsEqualTo(expected));
        }

        [TestMethod]
        public void Swap_FindMininums_test1()
        {
            var expected = new List<SwapFunc> { new SwapFunc(1, 2) };

            var searcher = new SwapListSearcher();
            var result = searcher.FindMininums(new List<int> { 1, 3, 2, 4 });

            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result.Any(p=>p.IsEqualTo(expected)));
        }

        [TestMethod]
        public void Swap_FindMininums_test2()
        {
            var expected1 = new List<SwapFunc> { new SwapFunc(0, 3), new SwapFunc(1, 2) };
            var expected2 = new List<SwapFunc> { new SwapFunc(1, 2), new SwapFunc(0, 3) };

            var searcher = new SwapListSearcher();
            var result = searcher.FindMininums(new List<int> { 4, 3, 2, 1 });

            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result.Any(p => p.IsEqualTo(expected1)));
            Assert.IsTrue(result.Any(p => p.IsEqualTo(expected2)));
        }

        [TestMethod]
        public void Swap_FindMininums_test3()
        {
            var searcher = new SwapListSearcher();
            var result = searcher.FindMininums(new List<int> { 4, 3, 1, 2 });

            Assert.IsTrue(result.Count == 6);            
        }

        [TestMethod]
        public void Swap_Consequtive()
        {            
            var list = new List<int> { 4, 3, 1, 2 };

            var tmp = list.ConsequitiveCombins();
            
        }

        [TestMethod]
        public void Swap_WriteToFile()
        {
            var searcher = new SwapListSearcher();
            var file = File.CreateText("Swaplists.txt");

            for (var i = 2; i <= 5; i++)
            {
                file.WriteLine($"List of {i}");
                file.WriteLine($"===========");
                Write(i, file);
                file.WriteLine();
            }

            file.Close();
        }

        public void Write(int lenght, StreamWriter file)
        {
            var searcher = new SwapListSearcher();            

            var lists = Enumerable.Range(1, lenght).Permute().Select(p => p.ToList()).ToList();

            foreach (var list in lists)
            {
                file.WriteLine($"{string.Join("", list)}");
                foreach (var swaps in searcher.FindMininums(list))
                {
                    var rotateCount = searcher.IsRotate(swaps, list.Count);
                    var rot = rotateCount == -1 ? "" : $" rot({rotateCount})";

                    var hasRotSize = searcher.HasRotate(swaps, list.Count);
                    var hasRotMsg = hasRotSize == -1 ? "" : $" has rot {hasRotSize}";

                    file.WriteLine($"   {string.Join(" ", swaps)} {rot} {hasRotMsg}");
                }
            }            
        }
    }

    public class SwapListSearcher
    {
        public int IsRotate(List<SwapFunc> swaps, int listLength)
        {
            var list = Enumerable.Range(1, listLength).ToList();

            var swapped = list.Swap(swaps);

            var copy = list.ToList();

            for (int i = 1; i <= listLength; i++)
            {
                copy = copy.RotateOne();

                if (swapped.SequenceEqual(copy))
                    return i;
            }

            return -1;
        }

        public int HasRotate(List<SwapFunc> swaps, int listLength)
        {
            var list = Enumerable.Range(1, listLength).ToList();

            var combins = swaps.ConsequitiveCombins();

            foreach(var combin in combins.OrderByDescending(p => p.Count))
            {
                var rot = IsRotate(combin, listLength);
                if (rot > 0)
                    return rot;
            }

            return -1;
        }

        public List<List<SwapFunc>> FindMininums(List<int> list)
        {
            var swapList = new List<List<SwapFunc>>();

            do
            {
                swapList = GetAllPossibleSwaps(list, swapList).ToList();
            }
            while (!swapList.Any(swaps => list.Swap(swaps).IsSorted()) && swapList.Count > 0);

            return swapList.Where(swaps => list.Swap(swaps).IsSorted()).ToList();
        }

        public IEnumerable<List<SwapFunc>> GetAllPossibleSwaps(List<int> list, List<List<SwapFunc>> swapLists)
        {
            if(swapLists.Count == 0)
            {
                foreach(var swap in GetAllPossibleSwaps(list))
                {
                    yield return new List<SwapFunc> { swap };
                }

                yield break;
            }

            foreach (var swapList in swapLists)
            {
                foreach (var pos in GetAllPossibleSwaps(list.Swap(swapList)))
                {
                    if(swapList.Last().A != pos.A)
                    { 
                        var newSwaps = new List<SwapFunc>();
                        newSwaps.AddRange(swapList);
                        newSwaps.Add(pos);

                        yield return newSwaps;
                    }
                }
            }
        }
        
        public IEnumerable<SwapFunc> GetAllPossibleSwaps(List<int> list)
        {
            for(var i = 0; i < list.Count - 1; i++)
            {
                for (var m = i; m < list.Count; m++)
                {
                    if (list[i] > list[m])
                        yield return new SwapFunc(i, m);
                }                 
            }            
        }
    }

    public class SwapFunc
    {
        public SwapFunc(int a, int b)
        {
            this.A = a;
            this.B = b;
        }

        public List<int> Run(List<int> list)
        {
            var copy = list.ToList();
            var tmp = copy[A];
            copy[A] = copy[B];
            copy[B] = tmp;

            return copy;
        }

        public int A { get; set; }
        public int B { get; set; }

        public bool Equals(SwapFunc other)
        {
            return this.A == other.A && this.B == other.B;
        }

        public override string ToString()
        {
            return $"sw({A}, {B})";
        }
    }

    public static class ListExt
    {
        public static bool IsSorted(this List<int> list)
        {
            for (var i = 0; i < list.Count - 1; i++)
            {
                if (list[i] > list[i + 1])
                    return false;
            }

            return true;
        }

        public static bool IsEqualTo(this List<SwapFunc> left, List<SwapFunc> right)
        {
            if (left.Count != right.Count)
            {
                return false;
            }

            for (var i = 0; i < left.Count; i++)
            {
                if (!left[i].Equals(right[i]))
                    return false;
            }

            return true;
        }

        public static List<int> Swap(this List<int> list, List<SwapFunc> swaps)
        {
            var copy = list.ToList();
            foreach (var sw in swaps)
            {
                copy = sw.Run(copy);
            }

            return copy;
        }

        //public static void Rotate(this List<int> list, int size)
        //{
        //    for (int i = 0; i < size; i++)
        //    {
        //        RotateOne(list);
        //    }
        //}

        public static List<int> RotateOne(this List<int> list)
        {
            var copy = list.ToList();

            var tmp = copy.First();
            copy.RemoveAt(0);
            copy.Add(tmp);

            return copy;
        }

        public static List<List<T>> ConsequitiveCombins<T>(this List<T> list)
        {
            var ret = new List<List<T>>();

            if (list.Count < 2)
                return ret;

            for (var size = 2; size < list.Count; size++)
            {
                for (var start = 0; start < list.Count - size + 1; start++)
                {
                    var tmp = new T[size];                    
                    list.CopyTo(start, tmp, 0, size);
                    ret.Add(tmp.ToList());
                }
            }

            return ret;
        }
    }

    public static class Permut
    {
        public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
            {
                yield break;
            }

            var list = sequence.ToList();

            if (!list.Any())
            {
                yield return Enumerable.Empty<T>();
            }
            else
            {
                var startingElementIndex = 0;

                foreach (var startingElement in list)
                {
                    var remainingItems = list.AllExcept(startingElementIndex);

                    foreach (var permutationOfRemainder in remainingItems.Permute())
                    {
                        yield return startingElement.Concat(permutationOfRemainder);
                    }

                    startingElementIndex++;
                }
            }
        }

        private static IEnumerable<T> Concat<T>(this T firstElement, IEnumerable<T> secondSequence)
        {
            yield return firstElement;
            if (secondSequence == null)
            {
                yield break;
            }

            foreach (var item in secondSequence)
            {
                yield return item;
            }
        }

        private static IEnumerable<T> AllExcept<T>(this IEnumerable<T> sequence, int indexToSkip)
        {
            if (sequence == null)
            {
                yield break;
            }

            var index = 0;

            foreach (var item in sequence.Where(item => index++ != indexToSkip))
            {
                yield return item;
            }
        }
    }
}
