using NUnit.Framework;

namespace TuringMachine.Tests.Head
{
    [TestFixture]
    public class WriteShould
    {
        [Test]
        public void ReturnNewTapeWithUpdatedHead()
        {
            var data = new[] {'a', 'b', 'c'};

            const string expected = "Head: a(f)c";
            var sut = new TuringMachine.Head(data, 1);
            var result = sut.Write('f');
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        public void NotMutateOriginalData()
        {
            var data = new[] { 'a', 'b', 'c' };

            const string expected = "Head: a(b)c";
            var sut = new TuringMachine.Head(data, 1);
            sut.Write('f');
            Assert.AreEqual(expected, sut.ToString());
        }
    }
}