using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ExprGenrator;
using NUnit.Framework;

namespace ExprGeneratorTest
{
    [TestFixture]
    public class ExpressionGeneratorTests
    {
        [Test]
        public void SortListOf1()
        {
            var theList = new[] { 9 };
            var expected = File.ReadAllLines(@"Resources\ListOf1Functions.txt");

            var solutions = GenerateDepthFirst(theList);
            var str = solutions.Select(p => p.ToString());
            Assert.IsTrue(expected.SequenceEqual(str));
        }

        [Test]
        public void SortListOf2()
        {
            var theList = new[] { 9, 5 };
            var expected = File.ReadAllLines(@"Resources\ListOf2Functions.txt");

            var solutions = GenerateDepthFirst(theList);
            var str = solutions.Select(p => p.ToString());
            Assert.IsTrue(expected.SequenceEqual(str));
        }

        [Test]
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
            var count = 1;
            var watch = new Stopwatch();
            watch.Start();

            var sort = new SortFunctionExpression();

            var generatedCount = 0;
            
            var solutions = new List<BaseFunction>();
            var tester = new SortTester(theList);
            var stateTester = new StateTester(theList);
            foreach (var func in sort.GenerateDepthFirst(context, null))
            {
                generatedCount++;

                if (!tester.Test(context, func)) continue;

                if (!stateTester.TestState(func, context)) continue;

                solutions.Add(func.Copy(null));

                Console.WriteLine($"GENERATED FUNCTION {count++} in {watch.Elapsed}:");
                Console.WriteLine(func.FullInfo(theList, context));
            }

            watch.Stop();
            Console.WriteLine("Total generated:" + generatedCount);
            Console.WriteLine($"Total time taken : {watch.Elapsed}ms");
            return solutions;
        }

        [Test]
        public void SortStatefullListOf1()
        {
            var theList = new[] { 9 };
            var expecteds = File.ReadAllLines(@"Resources\ListOf1Functions.txt");
            var expectedTotalGenerated = 1;

            var solutions = GenerateStateFull(theList, expectedTotalGenerated);
            var str = solutions.Select(p => p.ToString()).ToArray();
            
            Assert.AreEqual(expecteds.Length, str.Length);
            foreach (var expected in expecteds)
            {
                Assert.IsTrue(str.Contains(expected));
            }
        }

        [Test]
        public void SortStatefullListOf2()
        {
            var theList = new[] { 9, 5 };
            var expecteds = File.ReadAllLines(@"Resources\ListOf2Functions.txt");
            var expectedTotalGenerated = 2;

            var solutions = GenerateStateFull(theList, expectedTotalGenerated);
            var str = solutions.Select(p => p.ToString()).ToArray();

            Assert.AreEqual(expecteds.Length, str.Length);
            foreach (var expected in expecteds)
            {
                Assert.IsTrue(str.Contains(expected));
            }
        }

        [Test]
        public void SortStatefullListOf3()
        {
            var theList = new[] { 9, 5, 3 };
            var expecteds = File.ReadAllLines(@"Resources\ListOf3Functions.txt");
            var expectedTotalGenerated = 360;

            var solutions = GenerateStateFull(theList, expectedTotalGenerated);

            var str = solutions.Select(p => p.ToString()).ToArray();

            Assert.AreEqual(expecteds.Length, str.Length);
            foreach (var expected in expecteds)
            {
                Assert.IsTrue(str.Contains(expected));
            }
        }

        [Test]
        //[Ignore]
        public void SortStatefullListOf4()
        {
            var theList = new[] { 9, 5, 3, 2 };
            var expecteds = File.ReadAllLines(@"Resources\ListOf4Functions.txt");
            var expectedTotalGenerated = 11437027;

            var solutions = GenerateStateFull(theList, expectedTotalGenerated);
            
            var str = solutions.Select(p => p.ToString()).ToArray();

            //File.WriteAllLines(@"c:\src\tmpozz.txt", str);

            Assert.AreEqual(expecteds.Length, str.Length);
            foreach (var expected in expecteds)
            {
                Assert.IsTrue(str.Contains(expected));
            }
        }

        private static List<BaseFunction> GenerateStateFull(int[] theList, int expectedTotalGenerated)
        {
            var context = Context.CreateDefault(theList);
            var count = 1;
            var watch = new Stopwatch();
            watch.Start();

            var sort = new SortFunctionExpression();

            var generatedCount = 0;

            var solutions = new List<BaseFunction>();
            var tester = new SortTester(theList);
            var stateTester = new StateTester(theList);
            foreach (var func in sort.GenerateStateFull(context, null, false))
            {
                generatedCount++;

                if (!tester.TestStateFull(func)) continue;

                //if (!stateTester.TestState(func, context))
                //{
                //    Console.WriteLine($"Func {generatedCount} passed the sort test, but failed on state test"); 
                //    continue;
                //}

                solutions.Add(func.Copy(null));

                Console.WriteLine($"GENERATED FUNCTION {count++} in {watch.Elapsed}:");
                Console.WriteLine(func.FullInfo(theList, context));
            }

            watch.Stop();
            Console.WriteLine("Total generated:" + generatedCount);
            Console.WriteLine($"Total time taken : {watch.Elapsed}ms");

            Assert.AreEqual(expectedTotalGenerated, generatedCount);
            return solutions;
        }
    }
}
