using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OzCompress
{
    public class PrimeList
    {
        private readonly List<int> _primeList = 
            new List<int> { /*2,*/ 3, 5, 7, 11, 13, 17, 19, 23 };
        
        public void Run()
        {
            //CreateIndexFile(5, 100);
            //CreateIndexFile(7, 1000);
            //CreateIndexFile(11, 5000);
            //CreateIndexFile(13, 80000);
            //CreateIndexFile(17, 1200000);
            //CreateIndexFile(19, 30000000);
            CreateIndexFile(23,  520000000);
        }

        private void CreateIndexFile(int theNumber, int length)
        {
            var primes = _primeList.TakeWhile(p => p != theNumber).ToList();

            var list = new List<int>(length+1) { -1 };
            for (int i = 1; i < length; i=i+2)
            {
                list.Add(i);
            }

            list.RemoveDivisibles(primes);
            
            var indexes = GetDivisibleIndexes(theNumber, list);
            list.Clear();

            var repeating = indexes.FindRepeating();

            WriteToFile(theNumber, repeating);
        }

        private static List<int> GetDivisibleIndexes(int divisor, List<int> list)
        {
            int last = 0;
            var ret = new List<int>();

            for (int i=0; i< list.Count; i++)
            {
                if (!list[i].DivisibleBy(divisor)) continue;

                if (last == 0)//ignore first
                {
                    last = i;
                    continue;
                }

                ret.Add(i - last);
                last = i;
            }
            
            return ret;
        }

        private static void WriteToFile(int theNumber, List<int> list)
        {
            var fileName = "composites" + theNumber + ".txt";

            if (File.Exists(fileName))
                File.Delete(fileName);
            File.AppendAllLines(fileName, list.Select(p => p.ToString()));
        }


    }

    public static class IntExtensions
    {
        public static bool DivisibleBy(this int i, int divisor)
        {
            return i%divisor == 0;
        }
    }

    public static class ListExtensions
    {
        public static void RemoveDivisibles(this List<int> list, List<int> divisors)
        {
            foreach (var divisor in divisors)
                list.RemoveAll(p => p.DivisibleBy(divisor));
        }

        public static bool ContainsAt(this List<int> list, List<int> repeating, int start)
        {
            for (int i = 0; i < repeating.Count(); i++)
            {
                if (i + start > list.Count - 1)
                    return false;

                if (repeating[i] != list[i + start])
                    return false;
            }

            return true;
        }

        public static List<int> FindRepeating(this List<int> list)
        {
            for (int i = 2; i < list.Count / 2; i++)
            {
                var repeatingCandidate = list.Take(i).ToList();

                if (list.ContainsAt(repeatingCandidate, i))
                    return repeatingCandidate;
            }

            return new List<int>();
        }
    }
}
