using System.Linq;

namespace ExprGenrator
{
    public class SortTester
    {
        public static bool Test(Context context, BaseFunction func, int[] list)
        {
            foreach (var permuted in list.Permute())
            {
                ((ListConstant)context.List).Value = permuted.ToArray();
                var resultList = (int[])func.Eval();

                if (!StateTester.IsSorted(resultList)) return false;
            }

            return true;
        }
    }
}
