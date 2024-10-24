using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Singulink.Enums.Tests;

[PrefixTestClass]
public class EnumConvertTests
{
    [TestMethod]
    public new void ToString()
    {
        var converter = EnumConverter<NormalEnum>.Default;

        converter.AsString(NormalEnum.None).ShouldBe("None");
        converter.AsString(NormalEnum.B).ShouldBe("B");
        converter.AsString(NormalEnum.D).ShouldBe("D");
    }

    [TestMethod]
    public void UndefinedValueToString()
    {
        var converter = EnumConverter<NormalEnum>.Default;

        converter.AsString((NormalEnum)3).ShouldBe("3");
        converter.AsString((NormalEnum)12).ShouldBe("12");
    }

    [TestMethod]
    public void ToCustomNameString()
    {
        var converter = new EnumConverter<NormalEnum>(opt => opt.WithDisplayNameGetter());

        converter.AsString(NormalEnum.None).ShouldBe("None (Display)");
        converter.AsString(NormalEnum.C).ShouldBe("C (Display)");
    }

    [TestMethod]
    public void ToDefaultString()
    {
        default(NormalEnum).AsString().ShouldBe("None");
        default(NoDefaultEnum).AsString().ShouldBe("0");
    }

    [TestMethod]
    public void ParseWhitespace()
    {
        var converter = EnumConverter<NormalEnum>.Default;

        Should.Throw<FormatException>(() => converter.Parse(string.Empty));
        Should.Throw<FormatException>(() => converter.Parse(" "));
        Should.Throw<FormatException>(() => converter.Parse("     "));
    }

    [TestMethod]
    public void Parse()
    {
        var converter = EnumConverter<NormalEnum>.Default;

        converter.Parse("A").ShouldBe(NormalEnum.A);
        converter.Parse("C").ShouldBe(NormalEnum.C);
    }

    [TestMethod]
    public void ParseUndefined()
    {
        var converter = EnumConverter<NormalEnum>.Default;

        converter.Parse("32").ShouldBe((NormalEnum)32);
        converter.Parse("3").ShouldBe((NormalEnum)3);
    }

    [TestMethod]
    public void ParseMissing()
    {
        var converter = EnumConverter<NormalEnum>.Default;

        Should.Throw<FormatException>(() => converter.Parse("X"));
    }

    private enum NormalEnum : byte
    {
        [Display(Name = "None (Display)")]
        None = 0,
        [Display(Name = "A (Display)")]
        A = 1,
        [Display(Name = "B (Display)")]
        B = 2,
        [Display(Name = "C (Display)")]
        C = 4,
        [Display(Name = "D (Display)")]
        D = 8,
    }

    private enum NoDefaultEnum : long
    {
        A = 1,
        B = 2,
        C = 3,
    }
}
