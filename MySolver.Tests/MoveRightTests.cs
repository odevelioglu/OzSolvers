using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace MySolver.Tests
{
    [TestFixture]
    public class MoveRightTests
    {
        [Test]
        public void MoveRight_ShouldConvertAnyListToAnyOther()
        {
            var parameters = MoveRightParamGenerator.GetParams(4).ToList();

            var permuts = new List<int> { 1, 2, 3, 4 }.Permute().Select(p => p.ToArray()).ToList();

            foreach (var input in permuts)
            {
                foreach (var target in permuts)
                {
                    var converting = MoveRightParamGenerator.GetConvertingMoves(input, parameters, target);
                    Assert.IsNotNull(converting);
                }
            }
        }

        [Test]
        public void MoveRight_Misc()
        {
            Assert.IsTrue(MoveRightParamGenerator.GetParams(4).Count() == 24);

            Assert.IsTrue(new[] { 3, 2, 1 }.MoveRight(new[] { 2, 0, 1 }).SequenceEqual(new[] { 1, 2, 3 }));
        }
    }

    public class MoveRightParamGenerator
    {
        public static IEnumerable<int[]> GetParams(int listLength)
        {
            var current = new int[listLength];

            do
            {
                if (IsValid(current))
                    yield return current;
                current = Increment(current, 0);
            } while (current != null);
        }

        public static int[] GetConvertingMoves(int[] input, List<int[]> movesList, int[] target)
        {
            foreach (var moves in movesList)
            {
                var moved = input.MoveRight(moves);
                if (moved.SequenceEqual(target))
                    return moves;
            }

            return null;
        }

        private static int[] Increment(int[] toIncrement, int i)
        {
            if (i > toIncrement.Length - 1) return null;

            var copy = toIncrement.ToArray();

            copy[i]++;

            if (copy[i] < copy.Length)
                return copy;

            copy[i] = 0;

            return Increment(copy, ++i);
        }

        private static bool IsValid(int[] moves)
        {
            if (moves == null) return false;

            var posSet = new HashSet<int>();

            for (var i = 0; i < moves.Length; i++)
            {
                if (moves[i] > moves.Length) continue;

                var pos = (i + moves[i]) % moves.Length;

                if (posSet.Contains(pos))
                    return false;

                posSet.Add(pos);
            }

            return true;
        }
    }

    public static class ListExte
    {
        public static List<int> MoveRight(this List<int> list, int[] moves)
        {
            return MoveRight(list.ToArray(), moves).ToList();
        }

        public static int[] MoveRight(this int[] list, int[] moves)
        {
            var len = list.Length;
            Contract.Assert(len == moves.Length);

            var ret = new int[len];

            for (var i = 0; i < len; i++)
            {
                var position = (i + moves[i]) % len;
                ret[position] = list[i];
            }

            return ret;
        }
    }
}
