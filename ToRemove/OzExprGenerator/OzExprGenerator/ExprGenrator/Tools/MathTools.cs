using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExprGenrator
{
    public class MathTools
    {
        public static int Fact(int c)
        {
            var ret = 1;

            for (int i = 1; i <= c; i++)
            {
                ret = ret * i;
            }

            return ret;
        }
    }

    public static class ListExtensions
    {
        public static string GetHash(this List<int[]> list, FuncInfo info)
        {
            var str = new StringBuilder();
            str.Append(info.IfCount);
            str.Append("-");
            str.Append(info.SwapCount);
            str.Append("-");
            //str.Append(info.IfDepth);
            //str.Append("-");
            //str.Append(info.IfExecCount);

            foreach (var intse in list)
            {
                foreach (var i in intse)
                {
                    str.Append(i);
                    str.Append("-");
                }
            }

            return str.ToString();
        }

        public static List<int[]> Copy(this List<int[]> list)
        {
            var ret = new List<int[]>();
            foreach (var intse in list)
            {
                ret.Add((int[])intse.Clone());
            }

            return ret;
        }

        public static List<int[]>[] Copy(this List<int[]>[] list)
        {
            var ret = new List<int[]>[list.Length];
            for(int i = 0; i < list.Length; i++)
            {
                ret[i] = list[i].Copy();
            }

            return ret;
        }

        public static IEnumerable<int[]> Scope(this List<int[]> states, List<int> scope)
        {
            foreach (var orderNumber in scope)
            {
                yield return states[orderNumber];
            }
        }

        //public static HashSet<int> Copy(this HashSet<int> set)
        //{
        //    return new HashSet<int>(set.ToArray());
        //}

        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var intse in list)
            {
                action(intse);
            }
        }

        public static List<int> GetScopeSet(this List<int[]> list, List<int> scope, Func<int[], bool> predicate, out bool isChanged)
        {
            var ret = new List<int>();
            isChanged = false;

            foreach (var i in scope)
            {
                if (predicate(list[i]))
                    ret.Add(i);
                else
                    isChanged = true;
            }
            
            return ret;
        }
        
        public static int CombineToInt(this int[] list)
        {
            var ret = 0;
            var coef = 1;
            for (var i = list.Length - 1; i >= 0; i--)
            {
                ret = ret + coef * list[i];
                coef = coef * 10;
            }

            return ret;
        }
        
        public static int GetFitness(this int[] list)
        {
            var bad = 0;
            for (var i = 0; i < list.Length - 1; i++)
            {
                for (var j = i+1; j < list.Length; j++)
                {
                    if (list[i] > list[j]) bad++;
                }
            }
            return bad;
        }

        public static int[] CopyAndSwap(this int[] list, int i, int j)
        {
            var copy = list.ToArray();
            var tmp = copy[i];
            copy[i] = copy[j];
            copy[j] = tmp;
            return copy;
        }

        public static void Swap(this int[] list, int i, int j)
        {
            var tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
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
