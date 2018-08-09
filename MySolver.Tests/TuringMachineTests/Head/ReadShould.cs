using NUnit.Framework;

namespace TuringMachine.Tests.Head
{
    [TestFixture]
    public class ReadShould
    {
        [Test]
        public void ReturnTapeHead()
        {
            var data = new[] { 'a', 'b', 'c' };

            const char expected = 'b';
            var sut = new TuringMachine.Head(data, 1);
            var result = sut.Read();
            Assert.AreEqual(expected, result);
        }
    }
}