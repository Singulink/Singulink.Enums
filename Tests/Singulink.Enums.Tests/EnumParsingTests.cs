using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Singulink.Enums.Tests;

[PrefixTestClass]
public class EnumParsingTests
{
    [TestMethod]
    public new void ToString()
    {
        var parser = EnumConverter<NormalEnum>.Default;

        parser.AsString(NormalEnum.None).ShouldBe("None");
        parser.AsString(NormalEnum.B).ShouldBe("B");
        parser.AsString(NormalEnum.D).ShouldBe("D");
    }

    [TestMethod]
    public void UndefinedValueToString()
    {
        var parser = EnumConverter<NormalEnum>.Default;

        parser.AsString((NormalEnum)3).ShouldBe("3");
        parser.AsString((NormalEnum)12).ShouldBe("12");
    }

    [TestMethod]
    public void ToCustomNameString()
    {
        var parser = new EnumConverter<NormalEnum>(opt => opt.WithDisplayNameGetter());

        parser.AsString(NormalEnum.None).ShouldBe("None (Display)");
        parser.AsString(NormalEnum.C).ShouldBe("C (Display)");
    }

    [TestMethod]
    public void ParseWhitespace()
    {
        var parser = EnumConverter<NormalEnum>.Default;

        Should.Throw<FormatException>(() => parser.Parse(string.Empty));
        Should.Throw<FormatException>(() => parser.Parse(" "));
        Should.Throw<FormatException>(() => parser.Parse("     "));
    }

    [TestMethod]
    public void Parse()
    {
        var parser = EnumConverter<NormalEnum>.Default;

        parser.Parse("A").ShouldBe(NormalEnum.A);
        parser.Parse("C").ShouldBe(NormalEnum.C);
    }

    [TestMethod]
    public void ParseUndefined()
    {
        var parser = EnumConverter<NormalEnum>.Default;

        parser.Parse("32").ShouldBe((NormalEnum)32);
        parser.Parse("3").ShouldBe((NormalEnum)3);
    }

    [TestMethod]
    public void ParseMissing()
    {
        var parser = EnumConverter<NormalEnum>.Default;

        Should.Throw<FormatException>(() => parser.Parse("X"));
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
}
