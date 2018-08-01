using System;
using System.Collections.Generic;
using System.Linq;

namespace MySolver
{
    public static class MathHelper
    {
        public static double epsilon = 0.0001d;
        public static bool Eq(this double d1, double d2)
        {
            //if (double.IsInfinity(d1) || double.IsInfinity(d2)) return false;
            //if (double.IsNaN(d1) || double.IsNaN(d2)) return false;

            return Math.Abs(d1 - d2) < epsilon;
        }

        public static bool Gt(this double d1, double d2)
        {
            return d1 - d2 > epsilon;
        }

        public static bool Lt(this double d1, double d2)
        {
            return d2 - d1 > epsilon;
        }

        public static double Combin(int n, int k)
        {
            return Fact(n) / (Fact(n - k) * Fact(k));
        }

        private static readonly int[] cache = new int[11] { 1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880, 3628800 };
        public static int Fact(int n)
        {
            if (n <= 10)
                return cache[n];

            return n * Fact(n - 1);
        }

        // It makes 6 multiplications for n^15
        // But only 5 is necessary: n -> n^2 -> n^3 -> n^5 -> n^10 -> n^15
        // It's an open question
        public static long Pow(int n, int pow)
        {
            if (pow == 1) return n;
            
            if (pow % 2 == 0)
            {
                var x = Pow(n, pow / 2);
                return x * x;
            }

            var y = Pow(n, (pow - 1) / 2);
            return y * y * n;
        }

        public static IEnumerable<Tuple<T1, T2>> Enum<T1, T2>(List<T1> list1, List<T2> list2)
        {
            foreach (var elem1 in list1)
            {
                foreach (var elem2 in list2)
                {
                    yield return new Tuple<T1, T2>(elem1, elem2);
                }
            }
        }
        public static IEnumerable<Tuple<T1, T2, T3>> Enum<T1, T2, T3>(List<T1> list1, List<T2> list2, List<T3> list3)
        {
            foreach (var elem1 in list1)
            {
                foreach (var elem2 in list2)
                {
                    foreach (var elem3 in list3)
                    {
                        yield return new Tuple<T1, T2, T3>(elem1, elem2, elem3);
                    }
                }
            }
        }


        public static IEnumerable<int[]> Permut(IEnumerable<int[]> list)
        {
            var que = new Queue<int[]>();
            foreach (var variableInfo in list)
            {
                que.Enqueue(variableInfo);
            }

            return MathHelper.Permut(que);
        }

        public static IEnumerable<int[]> Permut(Queue<int[]> arrays)
        {
            var pick = arrays.Dequeue();

            if (arrays.Count == 0)
            {
                foreach (var elem in pick)
                {
                    yield return new[] { elem };
                }

                yield break;
            }
            
            foreach (var perm in Permut(arrays))
            {
                foreach (var elem in pick)
                {
                    yield return Concat(elem, perm);
                }
            }
        }

        public static int[] Concat(int d, int[] list)
        {
            var array = new int[list.Length + 1];
            array[0] = d;
            list.CopyTo(array, 1);

            return array;
        }
    }

    public static class ListExtensions
    {
        public static T[] First<T>(this T[] list, int n)
        {
            return list.Sub(0, n);
        }

        public static T[] Sub<T>(this T[] list, int start, int length)
        {
            var ret = new T[length];
            Array.Copy(list, start, ret, 0, length);
            return ret;
        }
    }    
}
