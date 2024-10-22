using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Singulink.Enums.Tests;

[PrefixTestClass]
public class FlagsParsingTests
{
    [TestMethod]
    public new void ToString()
    {
        var parser = EnumConverter<FlagsEnum>.Default;

        parser.AsString(FlagsEnum.All, SplitFlagsOptions.None).ShouldBe("All");
        parser.AsString(FlagsEnum.All, SplitFlagsOptions.AllMatchingFlags).ShouldBe("A, B, C, D, All");
        parser.AsString(FlagsEnum.A | FlagsEnum.D).ShouldBe("A, D");
    }

    [TestMethod]
    public void ToStringCustomSeparator()
    {
        var parser = new EnumConverter<FlagsEnum>(opt => opt.Separator = " | ");

        parser.AsString(FlagsEnum.All, SplitFlagsOptions.None).ShouldBe("All");
        parser.AsString(FlagsEnum.All, SplitFlagsOptions.AllMatchingFlags).ShouldBe("A | B | C | D | All");
        parser.AsString(FlagsEnum.A | FlagsEnum.D).ShouldBe("A | D");
    }

    [TestMethod]
    public void UndefinedValueToString()
    {
        var parser = EnumConverter<FlagsEnum>.Default;

        parser.AsString((FlagsEnum)16, SplitFlagsOptions.None).ShouldBe("16");
        parser.AsString(FlagsEnum.A | FlagsEnum.D | (FlagsEnum)16, SplitFlagsOptions.AllMatchingFlags).ShouldBe("A, D, 16");
        parser.AsString(FlagsEnum.All | (FlagsEnum)16, SplitFlagsOptions.AllMatchingFlags).ShouldBe("A, B, C, D, All, 16");
    }

    [TestMethod]
    public void ToCustomNameString()
    {
        var parser = new EnumConverter<FlagsEnum>(opt => opt.WithDisplayNameGetter());

        parser.AsString(FlagsEnum.A | FlagsEnum.D).ShouldBe("A (Display), D (Display)");
        parser.AsString(FlagsEnum.A | FlagsEnum.D, SplitFlagsOptions.None).ShouldBe("A (Display), D (Display)");
    }

    [TestMethod]
    public void Parse()
    {
        var parser = new EnumConverter<FlagsEnum>(opt => opt.Separator = " | ");

        parser.Parse("All").ShouldBe(FlagsEnum.All);
        parser.Parse("A | B | C | D | All").ShouldBe(FlagsEnum.All);
        parser.Parse("A|D").ShouldBe(FlagsEnum.A | FlagsEnum.D);
    }

    [TestMethod]
    public void ParseSpaceSeparator()
    {
        var parser = new EnumConverter<FlagsEnum>(opt => opt.Separator = " ");

        parser.Parse("All").ShouldBe(FlagsEnum.All);
        parser.Parse(" All ").ShouldBe(FlagsEnum.All);
        parser.Parse("  All  ").ShouldBe(FlagsEnum.All);
        parser.Parse("  A B C   D  All  ").ShouldBe(FlagsEnum.All);
        parser.Parse("A D").ShouldBe(FlagsEnum.A | FlagsEnum.D);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("     ")]
    public void ParseWhitespace(string s)
    {
        var parser = EnumConverter<FlagsEnum>.Default;
        Should.Throw<FormatException>(() => parser.Parse(s));

        parser = new EnumConverter<FlagsEnum>(opt => opt.Separator = " ");
        Should.Throw<FormatException>(() => parser.Parse(s));
    }

    [TestMethod]
    public void ParseCustomName()
    {
        var parser = new EnumConverter<FlagsEnum>(opt => opt.WithDisplayNameGetter());

        parser.Parse("All (Display)").ShouldBe(FlagsEnum.All);
        parser.Parse("A (Display)  , B (Display)").ShouldBe(FlagsEnum.A | FlagsEnum.B);
        parser.Parse("A (Display),D (Display),  32").ShouldBe(FlagsEnum.A | FlagsEnum.D | (FlagsEnum)32);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("  ")]
    [DataRow("  ,")]
    [DataRow(",  ")]
    [DataRow("   ")]
    public void InvalidSeparator(string separator)
    {
        Should.Throw<ArgumentException>(() => new EnumConverter<FlagsEnum>(opt => opt.Separator = separator));
    }

    [TestMethod]
    public void ParseInvalidSeparator()
    {
        var parser = EnumConverter<FlagsEnum>.Default;

        Should.Throw<FormatException>(() => parser.Parse("A,,B"));
        Should.Throw<FormatException>(() => parser.Parse(" , "));
        Should.Throw<FormatException>(() => parser.Parse(","));
        Should.Throw<FormatException>(() => parser.Parse("A,"));
        Should.Throw<FormatException>(() => parser.Parse("A, "));
        Should.Throw<FormatException>(() => parser.Parse(",B"));
        Should.Throw<FormatException>(() => parser.Parse(" ,B"));
    }

    [TestMethod]
    public void ParseIgnoreCase()
    {
        var result = Enum<AttributeTargets>.Parse("assembly, class, method", ignoreCase: true);
        result.ShouldBe(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method);
    }

    [Flags]
    private enum FlagsEnum : ushort
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
        [Display(Name = "All (Display)")]
        All = A | B | C | D,
    }
}
