using System;
using System.Collections.Generic;
using System.Linq;

namespace MySolver.Inferring
{
    public class RationalInferer
    {
        public Tuple<long, long> Infer(int[] list)
        {
            return this.Infer(list, 10);
        }

        public Tuple<long, long> Infer(int[] list, int bas)
        {
            var k = (int) Math.Ceiling(Math.Log(list.Length, bas)) * 2;
            
            // Skip the whole part
            return InferFromPart(list.Skip(1).Take(k).ToArray());
        }

        public Tuple<long, long> InferFromPart(int[] list)
        {
            return this.InferFromPart(list, 10);
        }

        public Tuple<long, long> InferFromPart(int[] list, int bas)
        {
            var f = SequenceHelper.ToLong(list);
            
            var convergent = new ConvergentCalculator();
            var denum = (decimal)Math.Pow(bas, list.Length);
            
            foreach (var d in ContinuedFraction.FractionIterator(f / denum))
            {
                convergent.Add(d);
                
                if (f == Math.Floor(denum * convergent.R / convergent.P))
                    return new Tuple<long, long>(convergent.R, convergent.P);
            }

            return null;
        }
    }

    public class ContinuedFraction
    {
        public long[] Coefs { get; set; }

        //https://en.wikipedia.org/wiki/Continued_fraction#Continued_fraction_expansions_of_.CF.80
        public static ContinuedFraction FromDecimal(decimal d)
        {
            return new ContinuedFraction {Coefs = FractionIterator(d).ToArray() };
        }

        public static IEnumerable<long> FractionIterator(decimal d)
        {
            var epsilon = (decimal)Math.Pow(10, -20);
            var trunc = new Func<decimal, long>(p => (long)Math.Truncate(p + epsilon));

            var firstWhole = trunc(d);
            yield return firstWhole;

            var number = d - firstWhole;
            while (number > epsilon)
            {
                var reciprocal = 1 / number;
                var whole = trunc(reciprocal);
                yield return whole;

                number = reciprocal - whole;
            }
        }

        public override string ToString()
        {
            return string.Join(", ", this.Coefs);
        }
    }

    public class ConvergentCalculator
    {
        public long R => this.cacheH.Last();
        public long P => this.cacheK.Last();

        public List<long> Coefs { get; }

        private readonly List<long> cacheH = new List<long> { 0, 1 };
        private readonly List<long> cacheK = new List<long> { 1, 0 };

        public ConvergentCalculator()
        {
            this.Coefs = new List<long>();
        }

        public void Add(long number)
        {
            this.Coefs.Add(number);
            var n = cacheH.Count;

            cacheH.Add(number * cacheH[n - 1] + cacheH[n - 2]);

            cacheK.Add(number * cacheK[n - 1] + cacheK[n - 2]);
        }

        public void Add(IEnumerable<long> numbers)
        {
            foreach (var number in numbers)
            {
                this.Add(number);
            }
        }
        
        //private static long FuncH(int n, long[] list)
        //{
        //    if (n == -2) return 0;
        //    if (n == -1) return 1;

        //    return list[n] * FuncH(n - 1, list) + FuncH(n - 2, list);
        //}

        //private static long FuncK(int n, long[] list)
        //{
        //    if (n == -2) return 1;
        //    if (n == -1) return 0;

        //    return list[n] * FuncK(n - 1, list) + FuncK(n - 2, list);
        //}
    }

    public class SequenceHelper
    {
        public static long ToLong(int[] list, int start, int length)
        {
            long ret = 0;
            for (var i = start; i < start + length; i++)
            {
                ret += list[length - i - 1] * (long)Math.Pow(10, i);
            }

            return ret;
        }

        public static long ToLong(int[] list)
        {
            return ToLong(list, 0, list.Length);
        }
    }




}
