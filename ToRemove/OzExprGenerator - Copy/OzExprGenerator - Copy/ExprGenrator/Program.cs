using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using ExprGenrator.Tools;

namespace ExprGenrator
{
    //https://cs.stackexchange.com/questions/43294/difference-between-small-and-big-step-operational-semantics
    // => Small-step semantics
    // => denotational semantics: As long as there are no side effects, no non-determinism and no functions, the denotational semantics of an expression is the value that it evaluates to.
    // => big-step semantics
    // “for all programs that the programmer can write” rather than “for all programs”


    // Consider expressions: A: ((5 + 7) + 3) B: ((5 + 5) + 5) C: ((1 + 2) + 1) D: ((2 + 1) + 1) 
    // small-step semantics would define a reduction chain like ((2+1)+1)→3+1→4 
    // big-step semantics would be very similar to a denotational semantics: ((2+1)+1)⇓3 vs [[((2+1)+1)]]=4

    // => http://computationbook.com/

    // =>https://www.syncfusion.com/resources/techportal/ebooks


    // context free grammar:
    // production rules
    // gen   -> sort(listC, block|nil) 
    // block -> block(void, block|nil) 
    // void  -> | if(bool, block)
    //          | swap(listC, intC, intC)
    // bool  -> greaterThan(int, int)
    // int   -> getValue(listC, intC)
    // intC  -> 0,1,2
    // listC -> {9,5,3}

    // NEw context free grammar:
    // production rules
    // gen   -> sort(listC, void) 
    // void  -> | if(bool, void) void
    //          | swap(listC, intC, intC) void
    //          | nil
    // bool  -> greaterThan(int, int)
    // int   -> getValue(listC, intC)
    // intC  -> 0,1,2
    // listC -> {9,5,3}

    public class Counts
    {
        public long Count;

        public int Block;
        public int Iff;
        public int Swap;
    }

    public class Counter
    {
        private static int N;

        private static int maxBlock;
        private static int maxIff;
        private static int maxSwap;
        public static long Count(int n)
        {
            N = n;
            maxBlock = MathTools.Fact(n);
            maxIff = n * (n - 1) / 2;
            maxSwap = n * (n - 1) / 2;

            var counts = new Counts();

            return Block(counts);
        }

        public static long Block(Counts counts)
        {
            if (counts.Block >= maxBlock)
                return 0;

            counts.Block++;

            var voids = Iff(counts) + Swap(counts);
            return voids +
                   Iff(counts) * Block(counts) +
                   Swap(counts) * Block(counts);
        }

        public static long Iff(Counts counts)
        {
            if (counts.Iff >= maxIff)
                return 0;

            counts.Iff++;

            return (N * (N - 1) / 2) *  Block(counts);
        }

        public static long Swap(Counts counts)
        {
            if (counts.Swap >= maxSwap)
                return 0;

            counts.Swap++;

            return N * (N - 1) / 2;
        }
    }
    
    class Program
    {
        
        static void Main(string[] args)
        {
            //var temp = Counter.Count(3);

            if (File.Exists("Generated.txt"))
                File.Delete("Generated.txt");
            
            var writer = File.AppendText("Generated.txt");

            var theList = new[] { 9, 5, 3, 1 };

            var context = Context.CreateDefault(theList);

            var watch = new Stopwatch();
            watch.Start();
            
            GenerateDepthFirst(writer, context, theList);

            watch.Stop();
            writer.WriteLine("Time taken:" + watch.ElapsedMilliseconds);
            writer.Dispose();
        }

        private static void GenerateDepthFirst(StreamWriter writer, Context context, int[] theList)
        {
            var count = 1;

            var sort = new SortFunctionExpression();

            var generatedCount = 0;

            foreach (var func in sort.GenerateDepthFirst(context, null)) //Exprs
            {
                generatedCount++;
                
                if (generatedCount % 1 == 0) Console.WriteLine("Generated: " + generatedCount);

                if (!SortTester.Test(context, func, (int[])theList.Clone())) continue;

                //if (func.TestState((int[])theList.Clone(), context))
                //{
                //    generatedCount--;
                //    continue;
                //}
                //if (func.Count<IfFunction>() != 3 || func.Count<SwapFunction>() != 3) continue;

                writer.WriteLine($"GENERATED FUNCTION {count++} :");
                writer.WriteLine(func.FullInfo(theList, context));
                writer.Flush();
            }

            //var iff = IfFunctionExpression.Cache;

            //foreach (var kvp in iff)
            //{
            //    writer.WriteLine("left: " + kvp.Key + " count: " + kvp.Value.Length);
            //    foreach (var fu in kvp.Value)
            //    {
            //        writer.WriteLine(fu.ToCSharpString());
            //        writer.WriteLine();
            //    }
            //}


            //var totiff = iff.Values.Sum(p => p.Length);
            //var block = BlockFunctionExpression.Cache;
            //var totblock = block.Values.Sum(p => p.Length);

            writer.WriteLine("Total generated:" + generatedCount);
        }
    }

   
}
