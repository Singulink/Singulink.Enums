using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Singulink.Enums.Tests;

[PrefixTestClass]
public class FlagsConvertTests
{
    [TestMethod]
    public new void ToString()
    {
        var converter = EnumConverter<FlagsEnum>.Default;

        converter.AsString(FlagsEnum.All, SplitFlagsOptions.None).ShouldBe("All");
        converter.AsString(FlagsEnum.All, SplitFlagsOptions.AllMatchingFlags).ShouldBe("A, B, C, D, All");
        converter.AsString(FlagsEnum.A | FlagsEnum.D).ShouldBe("A, D");
    }

    [TestMethod]
    public void ToStringCustomSeparator()
    {
        var converter = new EnumConverter<FlagsEnum>(opt => opt.Separator = " | ");

        converter.AsString(FlagsEnum.All, SplitFlagsOptions.None).ShouldBe("All");
        converter.AsString(FlagsEnum.All, SplitFlagsOptions.AllMatchingFlags).ShouldBe("A | B | C | D | All");
        converter.AsString(FlagsEnum.A | FlagsEnum.D).ShouldBe("A | D");
    }

    [TestMethod]
    public void UndefinedValueToString()
    {
        var converter = EnumConverter<FlagsEnum>.Default;

        converter.AsString((FlagsEnum)16, SplitFlagsOptions.None).ShouldBe("16");
        converter.AsString(FlagsEnum.A | FlagsEnum.D | (FlagsEnum)16, SplitFlagsOptions.AllMatchingFlags).ShouldBe("A, D, 16");
        converter.AsString(FlagsEnum.A | FlagsEnum.D | (FlagsEnum)16, SplitFlagsOptions.AllMatchingFlags | SplitFlagsOptions.ExcludeRemainder).ShouldBe("A, D");
        converter.AsString(FlagsEnum.All | (FlagsEnum)16, SplitFlagsOptions.AllMatchingFlags).ShouldBe("A, B, C, D, All, 16");
        converter.AsString(FlagsEnum.All | (FlagsEnum)16, SplitFlagsOptions.ExcludeRemainder).ShouldBe("All");
    }

    [TestMethod]
    public void ToCustomNameString()
    {
        var converter = new EnumConverter<FlagsEnum>(opt => opt.WithDisplayNameGetter());

        converter.AsString(FlagsEnum.A | FlagsEnum.D).ShouldBe("A (Display), D (Display)");
        converter.AsString(FlagsEnum.A | FlagsEnum.D, SplitFlagsOptions.None).ShouldBe("A (Display), D (Display)");
    }

    [TestMethod]
    public void ToDefaultString()
    {
        default(FlagsEnum).AsString().ShouldBe("None");
        ((FlagsEnum)16).AsString(SplitFlagsOptions.ExcludeRemainder).ShouldBe("None");

        default(NoDefaultFlags).AsString().ShouldBe("0");
        ((NoDefaultFlags)16).AsString(SplitFlagsOptions.ExcludeRemainder).ShouldBe("0");
    }

    [TestMethod]
    public void Parse()
    {
        var converter = new EnumConverter<FlagsEnum>(opt => opt.Separator = " | ");

        converter.Parse(" All").ShouldBe(FlagsEnum.All);
        converter.Parse("  A | B | C | D | All  ").ShouldBe(FlagsEnum.All);
        converter.Parse("A|D ").ShouldBe(FlagsEnum.A | FlagsEnum.D);
    }

    [TestMethod]
    public void ParseSpaceSeparator()
    {
        var converter = new EnumConverter<FlagsEnum>(opt => opt.Separator = " ");

        converter.Parse("All").ShouldBe(FlagsEnum.All);
        converter.Parse(" All ").ShouldBe(FlagsEnum.All);
        converter.Parse("  All  ").ShouldBe(FlagsEnum.All);
        converter.Parse("  A B C   D  All  ").ShouldBe(FlagsEnum.All);
        converter.Parse("A D").ShouldBe(FlagsEnum.A | FlagsEnum.D);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("     ")]
    public void ParseWhitespace(string s)
    {
        var converter = EnumConverter<FlagsEnum>.Default;
        Should.Throw<FormatException>(() => converter.Parse(s));

        converter = new EnumConverter<FlagsEnum>(opt => opt.Separator = " ");
        Should.Throw<FormatException>(() => converter.Parse(s));
    }

    [TestMethod]
    public void ParseCustomName()
    {
        var converter = new EnumConverter<FlagsEnum>(opt => opt.WithDisplayNameGetter());

        converter.Parse("All (Display)").ShouldBe(FlagsEnum.All);
        converter.Parse("A (Display)  , B (Display)").ShouldBe(FlagsEnum.A | FlagsEnum.B);
        converter.Parse("A (Display),D (Display),  32").ShouldBe(FlagsEnum.A | FlagsEnum.D | (FlagsEnum)32);
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
        var converter = EnumConverter<FlagsEnum>.Default;

        Should.Throw<FormatException>(() => converter.Parse("A,,B"));
        Should.Throw<FormatException>(() => converter.Parse(" , "));
        Should.Throw<FormatException>(() => converter.Parse(","));
        Should.Throw<FormatException>(() => converter.Parse("A,"));
        Should.Throw<FormatException>(() => converter.Parse("A, "));
        Should.Throw<FormatException>(() => converter.Parse(",B"));
        Should.Throw<FormatException>(() => converter.Parse(" ,B"));
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

    [Flags]
    private enum NoDefaultFlags : short
    {
        A = 1,
        B = 2,
        C = 4,
    }
}
