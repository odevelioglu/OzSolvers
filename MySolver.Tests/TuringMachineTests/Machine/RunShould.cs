using NUnit.Framework;

namespace TuringMachine.Tests.Machine
{
    [TestFixture]
    public class RunShould
    {
        [Test]
        public void AddTwoNumbers()
        {
            const string expected = "Head: (_)11111__";
            var sut = new TuringMachine.Machine(
                0,
                new TuringMachine.Head(new[] { '1', '1', '1', TuringMachine.Head.Blank, '1', '1' }, 0),
                TransitionTableGenerator.Addition());

            var result = sut.Run();
            Assert.AreEqual(expected, result.Head.ToString());
        }

        [Test]
        public void MultiplyTwoNumbers()
        {
            const string expected = "Head: ______(_)111111";
            var sut = new TuringMachine.Machine(
                0,
                new TuringMachine.Head(new[] { '1', '1', '1', TuringMachine.Head.Blank, '1', '1' }, 0),
                TransitionTableGenerator.Multiplication());

            var result = sut.Run();
            Assert.AreEqual(expected, result.Head.ToString());
        }

        [Test]
        public void ReturnOnError()
        {
            const string expected = "Head: (1)1_11";
            var sut = new TuringMachine.Machine(
                6,
                new TuringMachine.Head(new[] { '1', '1', TuringMachine.Head.Blank, '1', '1' }, 0),
                TransitionTableGenerator.Addition());

            var result = sut.Run();
            Assert.AreEqual(expected, result.Head.ToString());
            Assert.AreEqual(State.Error, result.State);
        }
    }
}