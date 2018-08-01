using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using ExprGenrator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExprGeneratorTest
{
    [TestClass]
    public class FitnessTests
    {
        [TestMethod]
        public void FitnessListOf3()
        {
            var list = new[] { 9, 5, 2 };
            var expectedFit = new[] { 3, 2, 2, 1, 1, 0 };

            var fits = new List<int>();
            foreach (var perm in list.Permute())
            {
                var fitness = perm.ToArray().GetFitness();
                fits.Add(fitness);
                Console.WriteLine(string.Join(",", perm) + " " + fitness);
            }

            Assert.IsTrue(expectedFit.SequenceEqual(fits));
        }

        [TestMethod]
        public void FitnessListOf4()
        {
            var list = new[] { 9, 5, 2, 1 };

            var fits = new List<Tuple<int[], int>>();
            foreach (var perm in list.Permute())
            {
                var fitness = perm.ToArray().GetFitness();
                fits.Add(new Tuple<int[], int>(perm.ToArray(), fitness));
            }

            foreach (var fit in fits.OrderByDescending(p => p.Item2))
            {
                Console.WriteLine(string.Join(",", fit.Item1) + " " + fit.Item2);
            }
        }

        [TestMethod]
        public void FitnessListOf4Swaptest()
        {
            var list = new[] { 9, 5, 2, 1 };

            var fits = new List<Tuple<int[], int>>();
            foreach (var perm in list.Permute())
            {
                var A = perm.ToArray().CopyAndSwap(0, 1);
                fits.Add(new Tuple<int[], int>(A, A.GetFitness()));
            }

            foreach (var fit in fits)
            {
                Console.WriteLine(string.Join(",", fit.Item1) + " " + fit.Item2);
            }
        }


        // GENERATED FUNCTION 788 :
        // 
        // void Sort(int[] A) // does this pass the statetest?
        // {
        //     if(A[0] > A[1])
        //     {
        //         Swap(A, 0, 1);
        //     }
        //     if(A[2] > A[3])
        //     {
        //         Swap(A, 2, 3);
        //     }
        //     if(A[0] > A[2])
        //     {
        //         Swap(A, 0, 2);
        //         Swap(A, 1, 3); // getting fitness better? 
        //     }
        //     if(A[1] > A[2])
        //     {
        //         Swap(A, 1, 2);
        //         if(A[2] > A[3])
        //         {
        //             Swap(A, 2, 3);
        //         }
        //     }
        // }
        // 
        // use array instead of List 
        // use statetest instead of fitness.

        [TestMethod]
        public void StateTest()
        {
            var theList = new[] { 9, 5, 2, 1 };

            var Swap = new Action<int[], int, int>((A, a, b) =>
            {
                //Assert.IsTrue(A[a] > A[b]);
                
                var tmp = A[b];
                A[b] = A[a];
                A[a] = tmp;
            });

            var prevStates = new List<int[]> { (int[])theList.Clone() };

            void AfterEval(int[] list)
            {
                if (prevStates.Any(l => l.SequenceEqual(list)))
                {
                    Assert.Fail("State failed!");
                }
                else
                {
                    prevStates.Add((int[])list.Clone());
                }
            }

            var Sort = new Action<int[]>(A =>
            {
                if (A[0] > A[1])
                {
                    Swap(A, 0, 1);
                    AfterEval(A);
                }
                if (A[2] > A[3])
                {
                    Swap(A, 2, 3);
                    AfterEval(A);
                }
                if (A[0] > A[2])
                {
                    Swap(A, 0, 2);
                    AfterEval(A);
                    Swap(A, 1, 3);
                    AfterEval(A);
                }
                if (A[1] > A[2])
                {
                    Swap(A, 1, 2);
                    AfterEval(A);
                    if (A[2] > A[3])
                    {
                        Swap(A, 2, 3);
                        AfterEval(A);
                    }
                }
            });

            //Expression<Action<int[]>> ttt = A => Sort(A);

            //StateTest
            foreach (var list in theList.Permute().Select(p => p.ToArray()))
            {
                var original = list.ToArray();

                prevStates = new List<int[]> { (int[])theList.Clone() };
                Sort(list);

                Assert.IsTrue(StateTester.IsSorted(list));
            }
        }

        [TestMethod]
        public void FitnessTestExpr()
        {
            var theList = new[] { 9, 5, 2, 1 };

            var Swap = new Action<int[], int, int>((A, a, b) =>
            {
                if (A[a] <= A[b])
                {
                    throw new Exception("Fitness wrong");
                }

                var tmp = A[b];
                A[b] = A[a];
                A[a] = tmp;
            });
            
            var Sort = new Action<int[]>(A =>
            {
                if (A[0] > A[1])
                {
                    Swap(A, 0, 1);
                }
                if (A[2] > A[3])
                {
                    Swap(A, 2, 3);
                }
                if (A[0] > A[2])
                {
                    Swap(A, 0, 2);
                    Swap(A, 1, 3); // decrement fitness for orig:5,2,9,1 from 2 to 1
                }
                if (A[1] > A[2])
                {
                    Swap(A, 1, 2);
                    if (A[2] > A[3])
                    {
                        Swap(A, 2, 3);
                    }
                }
            });

            //Expression<Action<int[]>> ttt = A => Sort(A);
            
            foreach (var list in theList.Permute().Select(p => p.ToArray()))
            {
                var original = list.ToArray();
                if (original.SequenceEqual(new[] { 5, 2, 9, 1 }) ||
                    original.SequenceEqual(new[] { 5, 2, 1, 9 }) ||
                    original.SequenceEqual(new[] { 2, 5, 9, 1 }) ||
                    original.SequenceEqual(new[] { 2, 5, 1, 9 }))
                {
                    Assert.ThrowsException<Exception>(() => Sort(list), "Ex");
                }
                else
                {
                    Sort(list);
                    Assert.IsTrue(StateTester.IsSorted(list));
                }
            }
        }

        [TestMethod]
        public void ExprGenerator()
        {
            var theList = new[] { 9, 5, 2, 1 };

            var context = Context.CreateDefault(theList);

            var gen = new ExpressionGenerator(context);

            var iff1 = gen.Iff(0, 1).In(gen.Swap(0, 1));
            var iff2 = gen.Iff(2, 3).In(gen.Swap(2, 3));
            iff1.Next(iff2);
            
            var iff3 = gen.Iff(0, 2).In(gen.Swap(0, 2).Next(gen.Swap(1, 3)));
            iff2.Next(iff3);

            var swap = gen.Swap(1, 2).Next( gen.Iff(2, 3).In(gen.Swap(2, 3)));
            
            var iff4 = gen.Iff(1, 2).In(swap);
            iff3.Next(iff4);

            var sort = gen.Sort().In(iff1);

            var stateTester = new StateTester(theList);
            if (!stateTester.TestState(sort, context))
            {
                
            }

            Console.WriteLine(sort.ToCSharpString());
            Console.WriteLine(sort.ToTreeString());

            //Console.WriteLine(sort.FullInfo(theList, context));
        }
    }

    public static class FuncExt
    {
        public static BaseFunction In(this BaseFunction parentFunc, BaseFunction innerfunc)
        {
            if (parentFunc is IfFunction)
            {
                parentFunc.ParamValues[1] = innerfunc;
                innerfunc.Parent = parentFunc;
                innerfunc.Info.States = parentFunc.Info.States;
            }

            if (parentFunc is SortFunction)
            {
                parentFunc.ParamValues[1] = innerfunc;
                innerfunc.Parent = parentFunc;
                innerfunc.Info.States = parentFunc.Info.States;
            }

            return parentFunc;
        }

        public static BaseFunction Next(this BaseFunction parentFunc, BaseFunction nextfunc)
        {
            nextfunc.Parent = parentFunc.Parent;
            parentFunc.Next = nextfunc;
            nextfunc.Info.States = parentFunc.Info.States;

            return parentFunc;
        }
    }

    public class ExpressionGenerator
    {
        private readonly Context context;

        public ExpressionGenerator(Context context)
        {
            this.context = context;
        }

        public BaseFunction Sort()
        {
            var sort = new SortFunction();
            sort.ParamValues[0] = context.List;
            //sort.ParamValues[1] = innerFunc;

            var list = ((ListConstant)context.List).Value;
            var states = new List<int[]>();
            foreach (var perm in list.Permute())
            {
                states.Add(perm.ToArray());
            }
            sort.Info.States = states;
            sort.Info.Scope = Enumerable.Range(0, sort.Info.States.Count).ToList();

            return sort;
        }

        public BaseFunction Swap(int a, int b)
        {
            var swap = new SwapFunction(null);
            swap.ParamValues[0] = context.List;
            swap.ParamValues[1] = this.context.Ints.Single(p=> ((IntConstant)p).Value ==  a);
            swap.ParamValues[2] = this.context.Ints.Single(p => ((IntConstant)p).Value == b);

            return swap;
        }

        public BaseFunction Iff(int a, int b)
        {
            var iff = new IfFunction(null);
            
            iff.ParamValues[0] = Gt(a, b, iff);
            //iff.ParamValues[1] = innerfunc;

            return iff;
        }

        public BaseFunction Gt(int a, int b, BaseFunction parent)
        {
            var gt = new GreaterThanFunction();
            gt.Parent = parent;
            gt.ParamValues[0] = Gv(a, gt);
            gt.ParamValues[1] = Gv(b, gt);

            return gt;
        }

        public BaseFunction Gv(int a, BaseFunction parent)
        {
            var gv = new GetValueFunction();
            gv.Parent = parent;
            gv.ParamValues[0] = context.List;
            gv.ParamValues[1] = this.context.Ints.Single(p => ((IntConstant)p).Value == a);

            return gv;
        }
    }
}
