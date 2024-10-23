using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CA1707 // Identifiers should not contain underscores

namespace Singulink.Enums.Tests;

[PrefixTestClass]
public class FlagsTests
{
    [TestMethod]
    public void AreFlagsDefined()
    {
        foreach (Flags f in Enum.GetValues(typeof(Flags)))
            f.AreFlagsDefined().ShouldBeTrue();

        ((Flags)16).AreFlagsDefined().ShouldBeFalse();
        EnumFlagsInfo<Flags>.AllDefinedFlags.ShouldBe(Flags.All);
    }

    [TestMethod]
    public void SplitAll()
    {
        var splitFlags = Flags.All.SplitFlags().ToList();
        splitFlags.ShouldBe([Flags.All]);

        splitFlags = Flags.All.SplitFlags(SplitFlagsOptions.AllMatchingFlags).ToList();
        splitFlags.ShouldBe([Flags.A, Flags.B, Flags.C, Flags.D, Flags.All]);
    }

    [TestMethod]
    public void SplitNone()
    {
        var splitFlags = Flags.None.SplitFlags().ToList();
        splitFlags.ShouldBe([Flags.None]);

        splitFlags = Flags.None.SplitFlags(SplitFlagsOptions.AllMatchingFlags).ToList();
        splitFlags.ShouldBe([Flags.None]);
    }

    [TestMethod]
    public void SplitNoDefault()
    {
        var splitFlags = default(NoDefaultFlags).SplitFlags().ToList();
        splitFlags.Count.ShouldBe(0);

        splitFlags = NoDefaultFlags.A.SplitFlags().ToList();
        splitFlags.ShouldBe([NoDefaultFlags.A]);
    }

    [TestMethod]
    public void HasAllFlags()
    {
        var value = Flags.A | Flags.B;

        value.HasAllFlags(Flags.A).ShouldBeTrue();
        value.HasAllFlags(Flags.B).ShouldBeTrue();
        value.HasAllFlags(Flags.A | Flags.B).ShouldBeTrue();
        value.HasAllFlags(Flags.A, Flags.B).ShouldBeTrue();
        value.HasAllFlags([Flags.A, Flags.B]).ShouldBeTrue();

        value.HasAllFlags(Flags.None).ShouldBeTrue();

        value.HasAllFlags(Flags.D).ShouldBeFalse();
        value.HasAllFlags(Flags.A, Flags.B, Flags.C).ShouldBeFalse();
        value.HasAllFlags([Flags.A, Flags.B, Flags.C]).ShouldBeFalse();
    }

    [TestMethod]
    public void HasAnyFlags()
    {
        var value = Flags.A | Flags.B;

        value.HasAnyFlag(Flags.A).ShouldBeTrue();
        value.HasAnyFlag(Flags.B).ShouldBeTrue();
        value.HasAnyFlag(Flags.A | Flags.B).ShouldBeTrue();
        value.HasAnyFlag(Flags.A, Flags.B).ShouldBeTrue();
        value.HasAnyFlag([Flags.A, Flags.B]).ShouldBeTrue();
        value.HasAnyFlag([Flags.A, Flags.B, Flags.C]).ShouldBeTrue();

        value.HasAnyFlag(Flags.None).ShouldBeFalse();

        value.HasAnyFlag(Flags.C).ShouldBeFalse();
        value.HasAnyFlag(Flags.C, Flags.D).ShouldBeFalse();
        value.HasAnyFlag([Flags.C, Flags.D]).ShouldBeFalse();
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

    [Flags]
    private enum NoDefaultFlags : short
    {
        A = 1,
        B = 2,
        C = 4,
        D = 8,
    }
}
