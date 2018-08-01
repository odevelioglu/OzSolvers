using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;

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

    //  http://www.nltk.org/_modules/nltk/parse/generate.html => generates all possible sentences of a grammar

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

    // using fitness function (num of swaps) doen't work. Ex 9,5,1 has fitness 1. if(A[0]>A[1]){ swap(A,0,1) } gives 5,9,1 fit=2
    // swap does not always get the fitness better
    // new fitness function works

    // recursive algo in parallel
    // L-systems -> inverse problem

    // check if depth, ifExecCount, SwapExecCount

    public class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists("Generated.txt"))
                File.Delete("Generated.txt");
            
            var writer = File.AppendText("Generated.txt");
            
            var theList = new[] { 9, 5, 3, 2 }; // list of 4: 12 results for buble sort

            var context = Context.CreateDefault(theList);
            
            GenerateDepthFirst(writer, context, theList);
            
            writer.Dispose();
        }
        
        private static void GenerateDepthFirst(StreamWriter writer, Context context, int[] theList)
        {
            var watch = new Stopwatch();
            watch.Start();
            var count = 1;
            
            var sort = new SortFunctionExpression();
            
            long generatedCount = 0;
            var tester = new SortTester(theList);
            var stateTester = new StateTester(theList);
            
            foreach (var func in sort.GenerateStateFull(context, null, false)) //Exprs
            {
                generatedCount++;
                
                if (generatedCount % 100000 == 0) Console.WriteLine("Generated: " + generatedCount);
                
                //if (!tester.Test(context, func)) continue;
                if(!tester.TestStateFull(func)) continue;

                if (!stateTester.TestState(func, context))
                {
                    writer.WriteLine("StateTest failed for the following function!");
                    //continue;
                }

                var msg = $"GENERATED FUNCTION {count++} in {watch.Elapsed}:";
                Console.WriteLine(msg);
                writer.WriteLine(msg);
                writer.WriteLine(func.FullInfo(theList, context));
                writer.Flush();
            }

            var tmp = SwapFunctionExpression.stateCache;


            writer.WriteLine("Total generated:" + generatedCount);

            watch.Stop();
            writer.WriteLine($"Total time taken : {watch.Elapsed}ms");
        }
    }
}
