﻿using NUnit.Framework;

namespace TuringMachine.Tests.Head
{
    [TestFixture]
    public class ToStringShould
    {
        [Test]
        public void PrintOutTape()
        {
            var data = new[] {'a', 'b', 'c', 'd', 'c', 'b', 'a'};

            var expected = "Head: abc(d)cba";
            var sut = new TuringMachine.Head(data, 3);
            Assert.AreEqual(expected, sut.ToString());

            expected = "Head: (a)bcdcba";
            sut = new TuringMachine.Head(data, 0);
            Assert.AreEqual(expected, sut.ToString());

            expected = "Head: abcdcb(a)";
            sut = new TuringMachine.Head(data, 6);
            Assert.AreEqual(expected, sut.ToString());
        }
    }
}