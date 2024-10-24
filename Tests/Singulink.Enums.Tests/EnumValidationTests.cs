using Microsoft.VisualStudio.TestTools.UnitTesting;

// #pragma warning disable CA1069 // Enums values should not be duplicated

namespace Singulink.Enums.Tests;

[PrefixTestClass]
public class EnumValidationTests
{
    [TestMethod]
    public void Continuous()
    {
        EnumRangeInfo<ContinuousA>.IsContinuous.ShouldBeTrue();
        EnumRangeInfo<ContinuousA>.DefinedMin.ShouldBe(ContinuousA.A);
        EnumRangeInfo<ContinuousA>.DefinedMax.ShouldBe(ContinuousA.E);

        foreach (ContinuousA value in Enum.GetValues(typeof(ContinuousA)))
        {
            value.IsDefined().ShouldBeTrue();
            value.IsValid().ShouldBeTrue();
        }

        (ContinuousA.A - 1).IsDefined().ShouldBeFalse();
        (ContinuousA.A - 1).IsValid().ShouldBeFalse();

        (ContinuousA.E + 1).IsDefined().ShouldBeFalse();
        (ContinuousA.E + 1).IsValid().ShouldBeFalse();
    }

    [TestMethod]
    public void ContinuousRandomOrderWithNegatives()
    {
        EnumRangeInfo<ContinuousB>.IsContinuous.ShouldBeTrue();
        EnumRangeInfo<ContinuousB>.DefinedMin.ShouldBe(ContinuousB.A);
        EnumRangeInfo<ContinuousB>.DefinedMax.ShouldBe(ContinuousB.E);

        foreach (ContinuousB value in Enum.GetValues(typeof(ContinuousB)))
        {
            value.IsDefined().ShouldBeTrue();
            value.IsValid().ShouldBeTrue();
        }

        (ContinuousB.A - 1).IsDefined().ShouldBeFalse();
        (ContinuousB.E + 1).IsDefined().ShouldBeFalse();
    }

    [TestMethod]
    public void Discontinuous()
    {
        EnumRangeInfo<Discontinous>.IsContinuous.ShouldBeFalse();
        EnumRangeInfo<Discontinous>.DefinedMin.ShouldBe(Discontinous.A);
        EnumRangeInfo<Discontinous>.DefinedMax.ShouldBe(Discontinous.E);

        foreach (Discontinous value in Enum.GetValues(typeof(Discontinous)))
        {
            value.IsDefined().ShouldBeTrue();
            value.IsValid().ShouldBe(value == Discontinous.B);
        }

        (Discontinous.A - 1).IsDefined().ShouldBeFalse();
        (Discontinous.A - 1).IsValid().ShouldBeFalse();

        (Discontinous.E + 1).IsDefined().ShouldBeFalse();
        (Discontinous.E + 1).IsValid().ShouldBeFalse();
    }

    [TestMethod]
    public void CustomValidator()
    {
        foreach (Discontinous value in Enum.GetValues(typeof(Discontinous)))
        {
            if ((long)value == 1)
                value.IsValid().ShouldBeTrue();
            else
                value.IsValid().ShouldBeFalse();
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
