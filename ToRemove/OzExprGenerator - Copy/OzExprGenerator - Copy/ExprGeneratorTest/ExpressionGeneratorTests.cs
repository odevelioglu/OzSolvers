using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ExprGenrator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExprGeneratorTest
{
    [TestClass]
    public class ExpressionGeneratorTests
    {
        [TestMethod]
        public void SortListOf1()
        {
            var theList = new[] { 9 };
            var expected = File.ReadAllLines(@"Resources\ListOf1Functions.txt");

            var solutions = GenerateDepthFirst(theList);
            var str = solutions.Select(p => p.ToString());
            Assert.IsTrue(expected.SequenceEqual(str));
        }

        [TestMethod]
        public void SortListOf2()
        {
            var theList = new[] { 9, 5 };
            var expected = File.ReadAllLines(@"Resources\ListOf2Functions.txt");

            var solutions = GenerateDepthFirst(theList);
            var str = solutions.Select(p => p.ToString());
            Assert.IsTrue(expected.SequenceEqual(str));
        }

        [TestMethod]
        public void SortListOf3()
        {
            var theList = new[] { 9, 5, 3 };
            var expected = File.ReadAllLines(@"Resources\ListOf3Functions.txt");

            var solutions = GenerateDepthFirst(theList);

            var str = solutions.Select(p => p.ToString());

            Assert.IsTrue(expected.SequenceEqual(str));
        }

        private static List<BaseFunction> GenerateDepthFirst(int[] theList)
        {
            var context = Context.CreateDefault(theList);

            var watch = new Stopwatch();
            watch.Start();

            var sort = new SortFunctionExpression();

            var generatedCount = 0;
            
            var solutions = new List<BaseFunction>();

            foreach (var func in sort.GenerateDepthFirst(context, null))
            {
                generatedCount++;

                if (!SortTester.Test(context, func, (int[])theList.Clone())) continue;

                if (!func.TestState((int[])theList.Clone(), context)) continue;

                solutions.Add(func.Copy(null));

                Console.WriteLine($"GENERATED FUNCTION {solutions.Count} :");
                Console.WriteLine(func.FullInfo(theList, context));
            }

            watch.Stop();
            Console.WriteLine("Total generated:" + generatedCount);
            Console.WriteLine("Time taken:" + watch.ElapsedMilliseconds);
            return solutions;
        }

        [TestMethod]
        public void SortListOf1Dyna()
        {
            var theList = new[] { 9 };
            var expected = File.ReadAllLines(@"Resources\ListOf1Functions.txt");

            var solutions = GenerateDepthFirstDyna(theList);
            var str = solutions.Select(p => p.ToString());
            Assert.IsTrue(expected.SequenceEqual(str));
        }

        [TestMethod]
        public void SortListOf2Dyna()
        {
            var theList = new[] { 9, 5 };
            var expected = File.ReadAllLines(@"Resources\ListOf2Functions.txt");

            var solutions = GenerateDepthFirstDyna(theList);
            var str = solutions.Select(p => p.ToString());
            Assert.IsTrue(expected.SequenceEqual(str));
        }

        [TestMethod]
        public void SortListOf3Dyna()
        {
            var theList = new[] { 9, 5, 4 };
            var expected = File.ReadAllLines(@"Resources\ListOf3Functions.txt");

            var solutions = GenerateDepthFirstDyna(theList);
            var str = solutions.Select(p => p.ToString());
            Assert.IsTrue(expected.SequenceEqual(str));
        }

        private static List<BaseFunction> GenerateDepthFirstDyna(int[] theList)
        {
            var context = Context.CreateDefault(theList);
            
            var watch = new Stopwatch();
            watch.Start();

            var sort = new SortFunctionExpression();

            var generatedCount = 0;

            var solutions = new List<BaseFunction>();

            foreach (var func in sort.GenerateDepthFirstDyna(context, null))
            {
                generatedCount++;

                if (!SortTester.Test(context, func, (int[])theList.Clone())) continue;

                if (!func.TestState((int[])theList.Clone(), context)) continue;

                solutions.Add(func.Copy(null));
                
                Console.WriteLine($"GENERATED FUNCTION {solutions.Count} :");
                Console.WriteLine(func.FullInfo(theList, context));
            }

            watch.Stop();
            Console.WriteLine("Total generated:" + generatedCount);
            Console.WriteLine("Time taken:" + watch.ElapsedMilliseconds);
            return solutions;
        }
    }
}
