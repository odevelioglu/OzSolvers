using NUnit.Framework;

namespace TuringMachine.Tests.Head
{
    [TestFixture]
    public class MoveHeadRightShould
    {
        [Test]
        public void MoveRightWhenAtLastHeadPostion()
        {
            const string expected = "Head: abcd(_)";
            var sut = new TuringMachine.Head(new[] { 'a', 'b', 'c', 'd' }, 3);
            var result = sut.MoveRight();
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        public void MoveToTheRight()
        {
            var data = new[] {'a', 'b', 'c', 'd', 'e'};

            var expected = "Head: abcd(e)";
            var sut = new TuringMachine.Head(data, 3);
            var result = sut.MoveRight();
            Assert.AreEqual(expected, result.ToString());

            expected = "Head: a(b)cde";
            sut = new TuringMachine.Head(data, 0);
            result = sut.MoveRight();
            Assert.AreEqual(expected, result.ToString());

            expected = "Head: ab(c)de";
            sut = new TuringMachine.Head(data, 1);
            result = sut.MoveRight();
            Assert.AreEqual(expected, result.ToString());
        }
    }
}