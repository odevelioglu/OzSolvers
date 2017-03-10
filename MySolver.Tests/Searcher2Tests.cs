using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using log4net;
using log4net.Config;
using System.Diagnostics;

namespace MySolver.Tests
{
    using System.Net;

    [TestClass]
    public class Searcher2Tests
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Searcher));

        private Stopwatch watch = new Stopwatch();
        
        [TestInitialize]
        public void Init()
        {
            //if(File.Exists("Log.log"))
            //    File.Delete("Log.log");
            XmlConfigurator.Configure();
            //Log.Info("Start");

            watch.Start();
        }

        [TestMethod]
        public void TestPeasantMultip()
        {
            // (10^m a + b) (10^m c + d) = 10^2m ac + 10^m (bc + ad) + bd 
            //                            "ac + bd − (a − b)(c − d) = bc + ad"
            //                           = 10^2m ac + 10^m (ac + bd − (a − b)(c − d)) + bd
            var funcs = new List<BinaryFunction> { Commons.add, Commons.sub, Commons.mul };
            var constants = new List<Constant> { new Constant(10), new Constant(100) };
            var toSolve = new Func<int[], double>(vars =>
            {
                var a = vars[0];
                var b = vars[1];
                var c = vars[2];
                var d = vars[3];
                return (10 * a + b) * (10 * c + d);
            });

            var variables = new List<VariableInfo>
            {
                new VariableInfo("a", new[] { 3, 2 }),
                new VariableInfo("b", new[] { 5, 4 }),
                new VariableInfo("c", new[] { 7, 6 }),
                new VariableInfo("d", new[] { 8, 9 }),
            };

            var searcher = new Searcher2(toSolve, variables, 5, funcs, constants);
            searcher.StopAtFirstResult = true;
            searcher.ResultFound += node => Console.WriteLine("Found: " + node);

            searcher.Search();
            Console.WriteLine("NumberOfVerifiedSolutions: " + searcher.NumberOfVerifiedSolutions);
            Console.WriteLine("Elapsed time: " + watch.ElapsedMilliseconds);

            Assert.AreEqual("((b + (10 * a)) * (d + (c * 10)))", searcher.FinalResults[0].ToString());
            //Assert.AreEqual("((10 ^ 2) * (a * c)) + (10 * ((b * c) + (a * d))) + (b * d) ", searcher.FinalResults[0].ToString());
        }

        [TestMethod]
        public void TestPeasantMultip_Half()
        {
            // (10^m a + b) (10^m c + d) = 10^2m ac + 10^m (bc + ad) + bd 
            //                            "ac + bd − (a − b)(c − d) = bc + ad"
            //                           = 100ac + 10(ac + bd − (a − b)(c − d)) + bd


            // 8li yi buldun. ac + bd − (a − b)(c − d). 
            // 14luyu bul

            var funcs = new List<BinaryFunction> { Commons.add, Commons.sub, Commons.mul };
            var constants = new List<Constant> {  };
            var toSolve = new Func<int[], double>(vars =>
            {
                var a = vars[0];
                var b = vars[1];
                var c = vars[2];
                var d = vars[3];
                return ( (10 * a + b) * (10 * c + d) - 100*a*c - b*d ) / 10;
            });
            
            var variables = new List<VariableInfo>
            {
                new VariableInfo("a", new[] { 3, 27 }),
                new VariableInfo("b", new[] { 5, 43 }),
                new VariableInfo("c", new[] { 7, 61 }),
                new VariableInfo("d", new[] { 8, 97  }),
            };

            var searcher = new Searcher2(toSolve, variables, 3, funcs, constants);
            
            searcher.ResultFound += node => Console.WriteLine("Found: " + node + " = " + node.Eval(new[] {3,5,7,8}) + " = " + toSolve(new[] { 3, 5, 7, 8 }));

            searcher.Search();
            Console.WriteLine("NumberOfVerifiedSolutions: " + searcher.NumberOfVerifiedSolutions);
            Console.WriteLine("Elapsed time: " + watch.ElapsedMilliseconds);
        }


    }
}
