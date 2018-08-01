using System.Linq;

namespace ExprGenrator
{
    public class SortTester
    {
        private readonly int[][] testCandidates;
        
        public SortTester(int[] list)
        {
            this.testCandidates = list.Permute().Select(p => p.ToArray()).ToArray();
        }

        public bool Test(Context context, BaseFunction func)
        {
            var initial = ((ListConstant)context.List).Value.ToArray();

            foreach (var testCandidate in this.testCandidates)
            {
                ((ListConstant)context.List).Value = testCandidate.ToArray(); // copy
                var resultList = (int[])func.Eval();

                if (!StateTester.IsSorted(resultList))
                {
                    ((ListConstant)context.List).Value = initial;
                    return false; // faster than sequance equal
                }
            }

            ((ListConstant)context.List).Value = initial;

            return true;
        }

        public bool TestStateFull(BaseFunction func)
        {
            foreach (var state in func.Info.States)
            {
                if (!StateTester.IsSorted(state))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
