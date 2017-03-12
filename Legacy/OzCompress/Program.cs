using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.IO;

namespace OzCompress
{
    class Program
    {
        private const string piFile = "Pi.txt";
        private const string s2File = "s2.txt";
        private const string s2FileBitArray = "s2BitArray.txt";

        private const string input =
            "0111011001111101100100010011010010000000000000101111100001101000100011111101010100111100111011";
        static void Main(string[] args)
        {
            //var test = new PrimeList();
            //test.Run();
            
            
            return;

            File.Delete("output.txt");

            //SaveS2InBitArray();

            //var toSearch = input.ToBitArray();
            var bytes = File.ReadAllBytes("s2ORG.bin");
            var toSearch = new BitArray(bytes).Sub(0, 4096*2);

            BitArray sRoot = null;
            using (var file = File.OpenText(s2FileBitArray))
            {
                sRoot = file.ReadToEnd().ToBitArray();
            }
            var index = 0;
            while (index < sRoot.Length - 20 && index < toSearch.Length)
            {
                var searchBits = toSearch.Sub(index, 20);
                var codon = Searcher.Search(sRoot, searchBits, index, 20);

                if (codon.Length >= 20)
                {
                    Write(codon + " " + sRoot.ToBitString(codon));
                    index += codon.Length;
                }
                else
                {
                    var shift = Searcher.SearchShift(sRoot, toSearch.Sub(index, 20), index, 20);
                    Write("Shifting {0} bits: ", shift);
                    index+=shift;
                }

                Console.WriteLine(index);
            }

            Console.ReadKey();
        }

        private static void Write(string text, params Object[] args)
        {
            using (var file = File.AppendText("output.txt"))
            {
                file.WriteLine(string.Format(text, args));
            }
        }
        
        public static void SavePI()
        {
            //PI
            File.Delete(piFile);

            var pi = GetPi(500);
            using (var file = File.AppendText(piFile))
            {
                file.Write(pi.ToString());
            }
        }

        public static void SaveS2InBitArray()
        {
            File.Delete(s2FileBitArray);
            HighPrecision.Precision = 100000;
            var s2 = SquareRoot(100000);
            
            using (var file = File.AppendText(s2FileBitArray))
            {
                file.Write(s2.Numerator.ToBinaryString());
            }
        }


       

        public static void SaveS2()
        {
            File.Delete(s2File);
            HighPrecision.Precision = 1000000;
            var s2 = SquareRoot(1000000);
            using (var file = File.AppendText(s2File))
            {
                file.Write(s2.ToString());
            }
        }

        public static HighPrecision GetPi(int digits)
        {
            HighPrecision.Precision = digits;
            HighPrecision first = 4 * Atan(5);
            HighPrecision second = Atan(239);
            return 4 * (first - second);
        }

        public static HighPrecision Atan(int denominator)
        {
            HighPrecision result = new HighPrecision(1, denominator);
            int xSquared = denominator * denominator;

            int divisor = 1;
            HighPrecision term = result;

            while (!term.IsZero)
            {
                divisor += 2;
                term /= xSquared;
                result -= term / divisor;

                divisor += 2;
                term /= xSquared;
                result += term / divisor;
            }

            return result;
        }

        //http://www.mathblog.dk/files/euler/Problem57.cs
        public static HighPrecision SquareRoot(int limit)
        {
            BigInteger den = 2;
            BigInteger num = 3;

            for (int i = 1; i < limit; i++)
            {
                num += 2 * den;
                den = num - den;
            }

            return new HighPrecision(num, den);
        }
    }
}
