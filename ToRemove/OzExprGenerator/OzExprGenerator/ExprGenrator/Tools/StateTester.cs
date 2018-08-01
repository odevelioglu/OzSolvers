using System;
using System.Collections.Generic;
using System.Linq;

namespace ExprGenrator
{
    public class StateTester
    {
        public static Action<BaseFunction, int[]> AfterEval;

        private readonly int[][] testCandidates;

        private readonly int[] expectedResult;
        public StateTester(int[] list)
        {
            this.testCandidates = list.Permute().Select(p => p.ToArray()).ToArray();
            this.expectedResult = list.OrderBy(p => p).ToArray();
        }

        public static bool IsSorted(int[] list)
        {
            for (int i = 0; i < list.Length - 1; i++)
            {
                if (list[i] > list[i + 1]) return false;
            }

            return true;
        }

        public bool TestState(BaseFunction func, Context context)
        {
            var initial = ((ListConstant)context.List).Value.ToArray();

            foreach (var candidate in testCandidates)
            {
                if (!InnerTestState(func, candidate.ToArray(), context))
                {
                    ((ListConstant)context.List).Value = initial;
                    return false;
                }
            }

            ((ListConstant)context.List).Value = initial;
            return true;
        }

        private bool InnerTestState(BaseFunction func, int[] testList, Context context)
        {
            var prevStates = new List<int[]> { (int[])testList.Clone() };
            if (IsSorted(testList))
                prevStates = new List<int[]>();

            var ret = true;

            AfterEval = (f, list) =>
            {
                if (prevStates.Any(l => l.SequenceEqual(list)))
                {
                    ret = false;
                }
                else
                {
                    prevStates.Add((int[])list.Clone());
                }
            };
            
            ((ListConstant)context.List).Value = testList;

            func.Eval();

            return ret;
        }
    }
}
