using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CA1069 // Enums values should not be duplicated

namespace Singulink.Enums.Tests
{
    [TestClass]
    public class EnumValidationTests
    {
        [TestMethod]
        public void Continuous()
        {
            Assert.IsTrue(EnumRangeInfo<ContinuousA>.IsContinuous);
            Assert.AreEqual(ContinuousA.A, EnumRangeInfo<ContinuousA>.DefinedMin);
            Assert.AreEqual(ContinuousA.E, EnumRangeInfo<ContinuousA>.DefinedMax);

            foreach (ContinuousA value in Enum.GetValues(typeof(ContinuousA))) {
                Assert.IsTrue(value.IsDefined());
                Assert.IsTrue(value.IsValid());
            }

            Assert.IsFalse((ContinuousA.A - 1).IsDefined());
            Assert.IsFalse((ContinuousA.E + 1).IsDefined());
        }

        [TestMethod]
        public void ContinuousRandomOrderWithNegatives()
        {
            Assert.IsTrue(EnumRangeInfo<ContinuousB>.IsContinuous);
            Assert.AreEqual(ContinuousB.A, EnumRangeInfo<ContinuousB>.DefinedMin);
            Assert.AreEqual(ContinuousB.E, EnumRangeInfo<ContinuousB>.DefinedMax);

            foreach (ContinuousB value in Enum.GetValues(typeof(ContinuousB))) {
                Assert.IsTrue(value.IsDefined());
                Assert.IsTrue(value.IsValid());
            }

            Assert.IsFalse((ContinuousB.A - 1).IsDefined());
            Assert.IsFalse((ContinuousB.E + 1).IsDefined());
        }

        [TestMethod]
        public void Discontinuous()
        {
            Assert.IsFalse(EnumRangeInfo<Discontinous>.IsContinuous);
            Assert.AreEqual(Discontinous.A, EnumRangeInfo<Discontinous>.DefinedMin);
            Assert.AreEqual(Discontinous.E, EnumRangeInfo<Discontinous>.DefinedMax);

            foreach (Discontinous value in Enum.GetValues(typeof(Discontinous)))
                Assert.IsTrue(value.IsDefined());

            Assert.IsFalse((Discontinous.A - 1).IsDefined());
            Assert.IsFalse((Discontinous.E + 1).IsDefined());
        }

        [TestMethod]
        public void CustomValidator()
        {
            foreach (Discontinous value in Enum.GetValues(typeof(Discontinous))) {
                if ((long)value == 1)
                    Assert.IsTrue(value.IsValid());
                else
                    Assert.IsFalse(value.IsValid());
            }
        }

        private enum ContinuousA
        {
            A,
            B,
            C,
            D,
            E,
        }

        private enum ContinuousB : sbyte
        {
            D = -8,
            A = -10,
            B = -9,
            E = -6,
            C = -7,
            C2 = -7,
            C3 = -7,
        }

        [Only1IsValidEnum]
        private enum Discontinous : long
        {
            A = 0,
            B = 1,
            E = 3,
            E2 = 3,
        }

        [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
        private class Only1IsValidEnumAttribute : Attribute, IEnumValidatorAttribute<Discontinous>
        {
            public bool IsValid(Discontinous value) => (long)value == 1;
        }
    }
}
