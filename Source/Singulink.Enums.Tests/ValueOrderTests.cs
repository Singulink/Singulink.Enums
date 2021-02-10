using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Singulink.Enums.Tests
{
    [TestClass]
    public class ValueOrderTests
    {
        [TestMethod]
        public void EnumOrder()
        {
            Assert.IsTrue(Enum<AnEnum>.Values.SequenceEqual(new[] { AnEnum.MinusTwo, AnEnum.MinusOne, AnEnum.Zero, AnEnum.One, AnEnum.Two }));
        }

        [TestMethod]
        public void FlagsOrder()
        {
            Assert.IsTrue(Enum<Flags>.Values.SequenceEqual(new[] { Flags.Zero, Flags.LowestBit, Flags.SecondLowestBit, Flags.HighestBit, Flags.Highest2Bits }));
        }

        private enum AnEnum
        {
            MinusTwo = -2,
            MinusOne = -1,
            Zero = 0,
            One = 1,
            Two = 2,
        }

        [Flags]
        private enum Flags
        {
            Zero = 0,
            LowestBit = 1,
            SecondLowestBit = 2,
            HighestBit = 1 << 31,
            Highest2Bits = 3 << 30,
        }
    }
}
