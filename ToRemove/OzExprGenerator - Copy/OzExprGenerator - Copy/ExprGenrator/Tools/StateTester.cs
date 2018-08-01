using System;
using System.Collections.Generic;
using System.Linq;

namespace ExprGenrator
{
    public static class StateTester
    {
        public static Action<BaseFunction, int[]> AfterEval;

        public static bool IsSorted(int[] list)
        {
            for (int i = 0; i < list.Length - 1; i++)
            {
                if (list[i] > list[i + 1]) return false;
            }

            return true;
        }

        public static bool TestState(this BaseFunction func, int[] list, Context context)
        {
            //return InnerTestState(func, list); //only worst

            foreach (var permuted in list.Permute())
            {
                if (!InnerTestState(func, permuted.ToArray(), context))
                    return false;
            }

            return true;
        }

        public static bool InnerTestState(BaseFunction func, int[] testList, Context context)
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

            // while evaluating, swap should not return to a previous state
            ((ListConstant)context.List).Value = (int[])testList.Clone();

            func.Eval();

            return ret;
        }
    }
}
