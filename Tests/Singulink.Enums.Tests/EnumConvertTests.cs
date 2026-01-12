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

    [TestMethod]
    public void WithRepeats()
    {
        var converterA = EnumConverter<WithRepeatsA>.Default;
        var converterB = EnumConverter<WithRepeatsB>.Default;

        converterA.AsString(WithRepeatsA.A).ShouldBe("A");
        converterA.AsString(WithRepeatsA.E).ShouldBe("E");
        converterA.AsString(WithRepeatsA.K).ShouldBe("K");
        converterA.AsString(WithRepeatsA.T).ShouldBe("T");
        converterA.AsString(WithRepeatsA.Y).ShouldBe("Y");

        converterB.AsString(WithRepeatsB.A).ShouldBe("A");
        converterB.AsString(WithRepeatsB.E).ShouldBe("E");
        converterB.AsString(WithRepeatsB.K).ShouldBe("K");
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

    private enum WithRepeatsA
    {
        A = 0,
        B = 0,
        C = 0,
        D = 0,
        E = 1,
        F = 1,
        G = 1,
        H = 1,
        I = 1,
        J = 1,
        K = 2,
        L = 2,
        M = 2,
        N = 2,
        O = 2,
        P = 2,
        Q = 2,
        R = 2,
        S = 2,
        T = 3,
        U = 3,
        V = 3,
        W = 3,
        X = 3,
        Y = 4,
        Z = 4,
    }

    private enum WithRepeatsB
    {
        A = 0,
        B = 0,
        C = 0,
        D = 0,
        E = 1,
        F = 1,
        G = 1,
        H = 1,
        I = 1,
        J = 1,
        K = 2,
        L = 2,
    }
}
