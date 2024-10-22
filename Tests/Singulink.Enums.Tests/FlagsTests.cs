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
            Assert.IsTrue(f.AreFlagsDefined());

        Assert.IsFalse(((Flags)16).AreFlagsDefined());
        Assert.AreEqual(Flags.All, EnumFlagsInfo<Flags>.AllDefinedFlags);
    }

    [TestMethod]
    public void SplitAll()
    {
        var splitFlags = Flags.All.SplitFlags().ToList();
        CollectionAssert.AreEqual(splitFlags, new[] { Flags.All });

        splitFlags = Flags.All.SplitFlags(SplitFlagsOptions.AllMatchingFlags).ToList();
        CollectionAssert.AreEquivalent(splitFlags, new[] { Flags.All, Flags.A, Flags.B, Flags.C, Flags.D });
    }

    [TestMethod]
    public void SplitNone()
    {
        var splitFlags = Flags.None.SplitFlags().ToList();
        CollectionAssert.AreEqual(splitFlags, new[] { Flags.None });

        splitFlags = Flags.None.SplitFlags(SplitFlagsOptions.AllMatchingFlags).ToList();
        CollectionAssert.AreEqual(splitFlags, new[] { Flags.None });
    }

    [TestMethod]
    public void HasAllFlags()
    {
        var value = Flags.A | Flags.B;

        Assert.IsTrue(value.HasAllFlags(Flags.A));
        Assert.IsTrue(value.HasAllFlags(Flags.B));
        Assert.IsTrue(value.HasAllFlags(Flags.A | Flags.B));
        Assert.IsTrue(value.HasAllFlags(Flags.A, Flags.B));
        Assert.IsTrue(value.HasAllFlags([Flags.A, Flags.B]));

        Assert.IsTrue(value.HasAllFlags(Flags.None));

        Assert.IsFalse(value.HasAllFlags(Flags.D));
        Assert.IsFalse(value.HasAllFlags(Flags.A, Flags.B, Flags.C));
        Assert.IsFalse(value.HasAllFlags([Flags.A, Flags.B, Flags.C]));
    }

    [TestMethod]
    public void HasAnyFlags()
    {
        var value = Flags.A | Flags.B;

        Assert.IsTrue(value.HasAnyFlag(Flags.A));
        Assert.IsTrue(value.HasAnyFlag(Flags.B));
        Assert.IsTrue(value.HasAnyFlag(Flags.A | Flags.B));
        Assert.IsTrue(value.HasAnyFlag(Flags.A, Flags.B));
        Assert.IsTrue(value.HasAnyFlag([Flags.A, Flags.B]));
        Assert.IsTrue(value.HasAnyFlag([Flags.A, Flags.B, Flags.C]));

        Assert.IsFalse(value.HasAnyFlag(Flags.None));

        Assert.IsFalse(value.HasAnyFlag(Flags.C));
        Assert.IsFalse(value.HasAnyFlag(Flags.C, Flags.D));
        Assert.IsFalse(value.HasAnyFlag([Flags.C, Flags.D]));
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
}
