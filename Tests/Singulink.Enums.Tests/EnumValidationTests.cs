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
        EnumRangeInfo<DiscontinuousA>.IsContinuous.ShouldBeFalse();
        EnumRangeInfo<DiscontinuousA>.DefinedMin.ShouldBe(DiscontinuousA.A);
        EnumRangeInfo<DiscontinuousA>.DefinedMax.ShouldBe(DiscontinuousA.E);

        foreach (DiscontinuousA value in Enum.GetValues(typeof(DiscontinuousA)))
        {
            value.IsDefined().ShouldBeTrue();
            value.IsValid().ShouldBe(value == DiscontinuousA.B);
        }

        (DiscontinuousA.A - 1).IsDefined().ShouldBeFalse();
        (DiscontinuousA.A - 1).IsValid().ShouldBeFalse();

        (DiscontinuousA.E + 1).IsDefined().ShouldBeFalse();
        (DiscontinuousA.E + 1).IsValid().ShouldBeFalse();
    }

    [TestMethod]
    public void CustomValidator()
    {
        foreach (DiscontinuousA value in Enum.GetValues(typeof(DiscontinuousA)))
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
    private enum DiscontinuousA : long
    {
        A = 0,
        B = 1,
        E = 3,
        E2 = 3,
    }

    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    private class Only1IsValidEnumAttribute : Attribute, IEnumValidatorAttribute<DiscontinuousA>
    {
        public bool IsValid(DiscontinuousA value) => (long)value == 1;
    }
}
