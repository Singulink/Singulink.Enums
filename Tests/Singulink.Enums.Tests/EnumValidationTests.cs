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

    [TestMethod]
    public void Unordered()
    {
        ((UnorderedA)(-2)).IsDefined().ShouldBeFalse();
        UnorderedA.ValueMinus1.IsDefined().ShouldBeTrue();
        UnorderedA.Value0.IsDefined().ShouldBeTrue();
        UnorderedA.Value1.IsDefined().ShouldBeTrue();
        UnorderedA.Value2.IsDefined().ShouldBeTrue();
        UnorderedA.Value3.IsDefined().ShouldBeTrue();
        UnorderedA.Value4.IsDefined().ShouldBeTrue();
        UnorderedA.Value5.IsDefined().ShouldBeTrue();
        UnorderedA.Value6.IsDefined().ShouldBeTrue();
        UnorderedA.Value7.IsDefined().ShouldBeTrue();
        UnorderedA.Value8.IsDefined().ShouldBeTrue();
        UnorderedA.Value9.IsDefined().ShouldBeTrue();
        UnorderedA.Value10.IsDefined().ShouldBeTrue();
        UnorderedA.Value11.IsDefined().ShouldBeTrue();
        UnorderedA.Value12.IsDefined().ShouldBeTrue();
        UnorderedA.Value13.IsDefined().ShouldBeTrue();
        UnorderedA.Value14.IsDefined().ShouldBeTrue();
        UnorderedA.Value15.IsDefined().ShouldBeTrue();
        UnorderedA.Value16.IsDefined().ShouldBeTrue();
        UnorderedA.Value17.IsDefined().ShouldBeTrue();
        UnorderedA.Value18.IsDefined().ShouldBeTrue();
        UnorderedA.Value19.IsDefined().ShouldBeTrue();
        UnorderedA.Value20.IsDefined().ShouldBeTrue();
        UnorderedA.Value21.IsDefined().ShouldBeTrue();
        UnorderedA.Value22.IsDefined().ShouldBeTrue();
        UnorderedA.Value23.IsDefined().ShouldBeTrue();
        UnorderedA.Value24.IsDefined().ShouldBeTrue();
        UnorderedA.Value25.IsDefined().ShouldBeTrue();
        UnorderedA.Value26.IsDefined().ShouldBeTrue();
        UnorderedA.Value27.IsDefined().ShouldBeTrue();
        UnorderedA.Value28.IsDefined().ShouldBeTrue();
        UnorderedA.Value29.IsDefined().ShouldBeTrue();
        ((UnorderedA)30).IsDefined().ShouldBeFalse();

        ((UnorderedB)(-2)).IsDefined().ShouldBeFalse();
        UnorderedB.ValueMinus1.IsDefined().ShouldBeTrue();
        UnorderedB.Value0.IsDefined().ShouldBeTrue();
        UnorderedB.Value1.IsDefined().ShouldBeTrue();
        UnorderedB.Value2.IsDefined().ShouldBeTrue();
        UnorderedB.Value3.IsDefined().ShouldBeTrue();
        UnorderedB.Value4.IsDefined().ShouldBeTrue();
        UnorderedB.Value5.IsDefined().ShouldBeTrue();
        UnorderedB.Value6.IsDefined().ShouldBeTrue();
        UnorderedB.Value7.IsDefined().ShouldBeTrue();
        UnorderedB.Value8.IsDefined().ShouldBeTrue();
        UnorderedB.Value9.IsDefined().ShouldBeTrue();
        ((UnorderedB)10).IsDefined().ShouldBeFalse();
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

    private enum UnorderedA
    {
        Value27 = 27,
        Value7 = 7,
        Value1 = 1,
        Value14 = 14,
        Value21 = 21,
        Value8 = 8,
        Value17 = 17,
        Value18 = 18,
        Value22 = 22,
        Value15 = 15,
        Value13 = 13,
        Value4 = 4,
        ValueMinus1 = -1,
        Value10 = 10,
        Value19 = 19,
        Value2 = 2,
        Value3 = 3,
        Value29 = 29,
        Value16 = 16,
        Value26 = 26,
        Value12 = 12,
        Value11 = 11,
        Value9 = 9,
        Value20 = 20,
        Value5 = 5,
        Value28 = 28,
        Value25 = 25,
        Value0 = 0,
        Value6 = 6,
        Value24 = 24,
        Value23 = 23,
    }

    private enum UnorderedB
    {
        Value7 = 7,
        Value1 = 1,
        Value8 = 8,
        Value4 = 4,
        ValueMinus1 = -1,
        Value2 = 2,
        Value3 = 3,
        Value9 = 9,
        Value5 = 5,
        Value0 = 0,
        Value6 = 6,
    }

    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    private class Only1IsValidEnumAttribute : Attribute, IEnumValidatorAttribute<DiscontinuousA>
    {
        public bool IsValid(DiscontinuousA value) => (long)value == 1;
    }
}
