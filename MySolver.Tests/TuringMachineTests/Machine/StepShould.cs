using NUnit.Framework;

namespace TuringMachine.Tests.Machine
{
    [TestFixture]
    public class StepShould
    {
        [Test]
        public void GoIntoErrorStateIfNoMatchingTransitions()
        {
            var sut = new TuringMachine.Machine(
                6,
                new TuringMachine.Head(new[] {'1', '1', '1'}, 1),
                TransitionTableGenerator.Addition());

            var result = sut.Step();
            Assert.AreEqual(State.Error, result.State);
        }

        [Test]
        public void ReturnThisIfHalted()
        {
            var sut = new TuringMachine.Machine(
                State.Halt,
                new TuringMachine.Head(new[] {'1', '1', '1'}, 0),
                TransitionTableGenerator.Addition());

            var result = sut.Step();
            Assert.AreEqual(State.Halt, result.State);
        }

        [Test]
        public void ReturnThisIfError()
        {
            var sut = new TuringMachine.Machine(
                State.Error,
                new TuringMachine.Head(new[] {'1', '1', '1'}, 0),
                TransitionTableGenerator.Addition());

            var result = sut.Step();
            Assert.AreEqual(State.Error, result.State);
        }

        [Test]
        public void MakeTransition()
        {
            var sut = new TuringMachine.Machine(
                0,
                new TuringMachine.Head(new[] {'1', '1', '1', TuringMachine.Head.Blank, '1', '1'}, 0),
                TransitionTableGenerator.Addition());

            var result = sut.Step();
            Assert.AreEqual(1, result.State);
            Assert.AreEqual('1', result.Head.Read());
        }

        [Test]
        public void WriteTransition()
        {
            var sut = new TuringMachine.Machine(
                3,
                new TuringMachine.Head(new[] { '1', '1', '1' }, 1),
                TransitionTableGenerator.Addition());
            
            var result = sut.Step().Head.MoveRight().Read();
            Assert.AreEqual(TuringMachine.Head.Blank, result);
        }
    }
}