using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable CS8605 // Unboxing a possibly null value.

namespace Singulink.Enums.Tests
{
    [TestClass]
    public class FlagsTests
    {
        [TestMethod]
        public void AreFlagsDefined()
        {
            foreach (Flags f in Enum.GetValues(typeof(Flags)))
                Assert.IsTrue(f.AreFlagsDefined());

            Assert.IsFalse(((Flags)16).AreFlagsDefined());
            Assert.AreEqual(Flags.All, EnumFlagsInfo<Flags>.AllDefinedFlags);
        }

        [TestMethod]
        public void SplitAll()
        {
            var splitFlags = Flags.All.SplitFlags(false).ToList();
            CollectionAssert.AreEqual(splitFlags, new[] { Flags.All });

            splitFlags = Flags.All.SplitFlags(true).ToList();
            CollectionAssert.AreEquivalent(splitFlags, new[] { Flags.All, Flags.A, Flags.B, Flags.C, Flags.D });
        }

        [TestMethod]
        public void SplitNone()
        {
            var splitFlags = Flags.None.SplitFlags(false).ToList();
            CollectionAssert.AreEqual(splitFlags, new[] { Flags.None });

            splitFlags = Flags.None.SplitFlags(true).ToList();
            CollectionAssert.AreEqual(splitFlags, new[] { Flags.None });
        }

        [Flags]
        private enum Flags : short
        {
            None = 0,
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            All = A | B | C | D,
        }
    }
}
