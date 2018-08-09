namespace TuringMachine.Tests.Head
{
    using NUnit.Framework;
    using System.Collections.Generic;

    [TestFixture]
    public class MoveHeadShould
    {
        private static readonly IEnumerable<char> Data = new[] { 'a', 'b', 'c', 'd', 'e' };

        [Test]
        public void MoveHeadLeftWithLeftDirection()
        {
            const string expected = "Head: ab(c)de";
            var sut = new TuringMachine.Head(Data, 3);
            var result = sut.Move(HeadDirection.Left);
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        public void MoveHeadRightWithRightDirection()
        {
            const string expected = "Head: ab(c)de";
            var sut = new TuringMachine.Head(Data, 1);
            var result = sut.Move(HeadDirection.Right);
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        public void NotMoveWithNoMove()
        {
            const string expected = "Head: ab(c)de";
            var sut = new TuringMachine.Head(Data, 2);
            var result = sut.Move(HeadDirection.NoMove);
            Assert.AreEqual(expected, result.ToString());
        }
    }
}