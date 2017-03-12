using System.Collections.Generic;
using System.Linq;

namespace OzAlgo
{
    public static class ListExtensions
    {
        public static bool HasSameElements(this List<int> list, List<int> that)
        {
            if (list.Except(that).Any())
                return false;

            if (that.Except(list).Any())
                return false;

            return true;
        }

        public static bool HasSameElements(this List<List<int>> list1, List<List<int>> list2)
        {
            if (list1.Count != list2.Count)
                return false;

            var comparer = new ListEqualityComparer();

            if (!list1.All(subList1 => list2.Contains(subList1, comparer)))
                return false;

            if (!list2.All(subList2 => list1.Contains(subList2, comparer)))
                return false;

            return true;
        }

        public static bool IsReverse(this List<List<int>> list1, List<List<int>> list2)
        {
            if (list1.Count != list2.Count)
                return false;
            
            int count = list1.Count;
            for (int i = 0; i < count; i++)
            {
                if (!list1[i].HasSameElements(list2[count - i - 1]))
                    return false;
            }

            return true;
        }

        public static bool SequenceEqualList(this List<List<int>> list1, List<List<int>> list2)
        {
            if (list1.Count != list2.Count)
                return false;

            for (var i = 0;  i < list1.Count; i++)
            {
                if(!list1[i].SequenceEqual(list2[i]))
                    return false;
            }

            return true;
        }
    }

    public class ListEqualityComparer : IEqualityComparer<List<int>>
    {
        public bool Equals(List<int> x, List<int> y)
        {
            return x.HasSameElements(y);
        }

        public int GetHashCode(List<int> obj)
        {
            return obj.GetHashCode();
        }
    }

    public class OrderedListEqualityComparer : IEqualityComparer<List<int>>
    {
        public bool Equals(List<int> x, List<int> y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(List<int> obj)
        {
            int res = 0x2D2816FE;
            foreach (var item in obj)
            {
                res = res * 31 + (item == null ? 0 : item.GetHashCode());
            }
            return res;
        }
    }
}
