using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Singulink.Enums.Tests;

[PrefixTestClass]
public class EnumFormatTests
{
    private static readonly CultureInfo _english = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo _french = CultureInfo.GetCultureInfo("fr-FR");

    [TestMethod]
    public void ToStringWithFormats()
    {
        Status.Active.ToString(EnumFormat.Name).ShouldBe("Active");
        Status.Active.ToString(EnumFormat.DisplayName).ShouldBe("Active (Display)");
        Status.Active.ToString(EnumFormat.Description).ShouldBe("Active (Description)");
        Status.Active.ToString(EnumFormat.EnumMemberValue).ShouldBe("active");
    }

    [TestMethod]
    public void ToStringFallsBackToFieldName()
    {
        Status.Unattributed.ToString(EnumFormat.Name).ShouldBe("Unattributed");
        Status.Unattributed.ToString(EnumFormat.DisplayName).ShouldBe("Unattributed");
        Status.Unattributed.ToString(EnumFormat.Description).ShouldBe("Unattributed");
        Status.Unattributed.ToString(EnumFormat.EnumMemberValue).ShouldBe("Unattributed");
    }

    [TestMethod]
    public void ToStringUndefinedValue()
    {
        ((Status)42).ToString(EnumFormat.Name).ShouldBe("42");
        ((Status)42).ToString(EnumFormat.DisplayName).ShouldBe("42");
        ((Status)42).ToString(EnumFormat.Description).ShouldBe("42");
        ((Status)42).ToString(EnumFormat.EnumMemberValue).ShouldBe("42");
    }

    [TestMethod]
    public void ToStringInvalidFormat()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Status.Active.ToString((EnumFormat)100));
        Should.Throw<ArgumentOutOfRangeException>(() => EnumConverter<Status>.GetDefault((EnumFormat)100));
    }

    [TestMethod]
    public void ToStringFlags()
    {
        (Options.Alpha | Options.Beta).ToString(EnumFormat.DisplayName).ShouldBe("Alpha (Display), Beta (Display)");
        (Options.Alpha | Options.Beta).ToString(EnumFormat.Description).ShouldBe("Alpha (Description), Beta (Description)");
        (Options.Alpha | Options.Beta).ToString(EnumFormat.EnumMemberValue).ShouldBe("alpha, beta");

        Options.All.ToString(EnumFormat.DisplayName).ShouldBe("All (Display)");
        Options.All.ToString(EnumFormat.DisplayName, SplitFlagsOptions.AllMatchingFlags)
            .ShouldBe("Alpha (Display), Beta (Display), Gamma (Display), All (Display)");
        Options.All.ToString(SplitFlagsOptions.AllMatchingFlags).ShouldBe("Alpha, Beta, Gamma, All");
    }

    [TestMethod]
    public void ToStringExtensionDiffersFromBuiltIn()
    {
        var value = Options.Alpha | (Options)16;

        value.ToString().ShouldBe("17");
        value.ToString(EnumFormat.Name).ShouldBe("Alpha, 16");
        value.ToString(SplitFlagsOptions.None).ShouldBe("Alpha, 16");
        value.ToString(EnumFormat.Name, SplitFlagsOptions.ExcludeRemainder).ShouldBe("Alpha");
    }

    [TestMethod]
    public void GetNameWithFormats()
    {
        Status.Active.GetName(EnumFormat.Name).ShouldBe("Active");
        Status.Active.GetName(EnumFormat.DisplayName).ShouldBe("Active (Display)");
        Status.Active.GetName(EnumFormat.Description).ShouldBe("Active (Description)");
        Status.Active.GetName(EnumFormat.EnumMemberValue).ShouldBe("active");

        Should.Throw<MissingMemberException>(() => ((Status)42).GetName(EnumFormat.DisplayName));

        Status.Active.TryGetName(EnumFormat.DisplayName, out string? name).ShouldBeTrue();
        name.ShouldBe("Active (Display)");

        ((Status)42).TryGetName(EnumFormat.DisplayName, out name).ShouldBeFalse();
        name.ShouldBeNull();
    }

    [TestMethod]
    public void ParseWithFormats()
    {
        Enum<Status>.Parse("Active", EnumFormat.Name).ShouldBe(Status.Active);
        Enum<Status>.Parse("Active (Display)", EnumFormat.DisplayName).ShouldBe(Status.Active);
        Enum<Status>.Parse("Active (Description)", EnumFormat.Description).ShouldBe(Status.Active);
        Enum<Status>.Parse("active", EnumFormat.EnumMemberValue).ShouldBe(Status.Active);
        Enum<Status>.Parse("Unattributed", EnumFormat.EnumMemberValue).ShouldBe(Status.Unattributed);

        Enum<Status>.Parse("Active (Display)".AsSpan(), EnumFormat.DisplayName).ShouldBe(Status.Active);

        Should.Throw<FormatException>(() => Enum<Status>.Parse("Active", EnumFormat.DisplayName));
        Should.Throw<FormatException>(() => Enum<Status>.Parse("Active (Display)", EnumFormat.Name));
    }

    [TestMethod]
    public void ParseWithFormatsIgnoreCase()
    {
        Should.Throw<FormatException>(() => Enum<Status>.Parse("ACTIVE (DISPLAY)", EnumFormat.DisplayName));
        Enum<Status>.Parse("ACTIVE (DISPLAY)", EnumFormat.DisplayName, ignoreCase: true).ShouldBe(Status.Active);
        Enum<Status>.Parse("ACTIVE (DISPLAY)".AsSpan(), EnumFormat.DisplayName, ignoreCase: true).ShouldBe(Status.Active);

        Enum<Status>.TryParse("ACTIVE", EnumFormat.EnumMemberValue, out var value).ShouldBeFalse();
        Enum<Status>.TryParse("ACTIVE", EnumFormat.EnumMemberValue, ignoreCase: true, out value).ShouldBeTrue();
        value.ShouldBe(Status.Active);
    }

    [TestMethod]
    public void ParseFlagsWithFormats()
    {
        Enum<Options>.Parse("alpha, beta", EnumFormat.EnumMemberValue).ShouldBe(Options.Alpha | Options.Beta);
        Enum<Options>.Parse("Alpha (Display), 16", EnumFormat.DisplayName).ShouldBe(Options.Alpha | (Options)16);
    }

    [TestMethod]
    public void GetValueWithFormats()
    {
        Enum<Status>.GetValue("Active (Display)", EnumFormat.DisplayName).ShouldBe(Status.Active);
        Enum<Status>.GetValue("Active (Display)".AsSpan(), EnumFormat.DisplayName).ShouldBe(Status.Active);
        Enum<Status>.GetValue("active (display)", EnumFormat.DisplayName, ignoreCase: true).ShouldBe(Status.Active);

        Should.Throw<MissingMemberException>(() => Enum<Status>.GetValue("Active", EnumFormat.DisplayName));

        Enum<Status>.TryGetValue("Active (Display)", EnumFormat.DisplayName, out var value).ShouldBeTrue();
        value.ShouldBe(Status.Active);

        Enum<Status>.TryGetValue("active (display)", EnumFormat.DisplayName, out value).ShouldBeFalse();
        Enum<Status>.TryGetValue("active (display)", EnumFormat.DisplayName, ignoreCase: true, out value).ShouldBeTrue();
        value.ShouldBe(Status.Active);
    }

    [TestMethod]
    public void GetDefaultReturnsSharedConverters()
    {
        EnumConverter<Status>.GetDefault(EnumFormat.Name).ShouldBeSameAs(EnumConverter<Status>.Default);
        EnumConverter<Status>.GetDefault(EnumFormat.Name, ignoreCase: true).ShouldBeSameAs(EnumConverter<Status>.DefaultIgnoreCase);

        foreach (var format in new[] { EnumFormat.DisplayName, EnumFormat.Description, EnumFormat.EnumMemberValue })
        {
            var converter = EnumConverter<Status>.GetDefault(format);
            var converterIgnoreCase = EnumConverter<Status>.GetDefault(format, ignoreCase: true);

            converter.ShouldNotBeSameAs(converterIgnoreCase);
            EnumConverter<Status>.GetDefault(format).ShouldBeSameAs(converter);
            EnumConverter<Status>.GetDefault(format, ignoreCase: true).ShouldBeSameAs(converterIgnoreCase);
        }
    }

    [TestMethod]
    public void CultureIgnoredForCultureInsensitiveFormats()
    {
        // Culture only matters for display names that are localized via resources, so every culture should share the same converter otherwise.

        foreach (var format in new[] { EnumFormat.Name, EnumFormat.DisplayName, EnumFormat.Description, EnumFormat.EnumMemberValue })
        {
            var converter = EnumConverter<Status>.GetDefault(format);
            EnumConverter<Status>.GetDefault(format, _english).ShouldBeSameAs(converter);
            EnumConverter<Status>.GetDefault(format, _french).ShouldBeSameAs(converter);
            EnumConverter<Status>.GetDefault(format, new CultureInfo("fr-FR")).ShouldBeSameAs(converter);
        }

        RunWithUICulture(_french, () => Status.Active.ToString(EnumFormat.DisplayName).ShouldBe("Active (Display)"));
        Status.Active.ToString(EnumFormat.DisplayName, _french).ShouldBe("Active (Display)");
    }

    [TestMethod]
    public void LocalizedDisplayNameUsesCurrentUICulture()
    {
        RunWithUICulture(_english, () => {
            Localized.Yes.ToString(EnumFormat.DisplayName).ShouldBe("Yes");
            Localized.No.ToString(EnumFormat.DisplayName).ShouldBe("No");
            Localized.Maybe.ToString(EnumFormat.DisplayName).ShouldBe("Maybe");
            Enum<Localized>.Parse("Yes", EnumFormat.DisplayName).ShouldBe(Localized.Yes);
            Enum<Localized>.TryParse("Oui", EnumFormat.DisplayName, out _).ShouldBeFalse();
        });

        RunWithUICulture(_french, () => {
            Localized.Yes.ToString(EnumFormat.DisplayName).ShouldBe("Oui");
            Localized.No.ToString(EnumFormat.DisplayName).ShouldBe("Non");
            Localized.Maybe.ToString(EnumFormat.DisplayName).ShouldBe("Maybe");
            Enum<Localized>.Parse("Oui", EnumFormat.DisplayName).ShouldBe(Localized.Yes);
            Enum<Localized>.TryParse("Yes", EnumFormat.DisplayName, out _).ShouldBeFalse();
        });
    }

    [TestMethod]
    public void LocalizedDisplayNameWithExplicitCulture()
    {
        RunWithUICulture(_english, () => {
            Localized.Yes.ToString(EnumFormat.DisplayName, _french).ShouldBe("Oui");
            Localized.Yes.ToString(EnumFormat.DisplayName, _english).ShouldBe("Yes");
            Enum<Localized>.Parse("Oui", EnumFormat.DisplayName, _french).ShouldBe(Localized.Yes);
            Enum<Localized>.Parse("Oui".AsSpan(), EnumFormat.DisplayName, _french).ShouldBe(Localized.Yes);
            Enum<Localized>.Parse("oui", EnumFormat.DisplayName, _french, ignoreCase: true).ShouldBe(Localized.Yes);
            Enum<Localized>.GetValue("Oui", EnumFormat.DisplayName, _french).ShouldBe(Localized.Yes);
            Enum<Localized>.GetValue("oui".AsSpan(), EnumFormat.DisplayName, _french, ignoreCase: true).ShouldBe(Localized.Yes);

            Enum<Localized>.TryParse("Oui", EnumFormat.DisplayName, _french, ignoreCase: false, out var value).ShouldBeTrue();
            value.ShouldBe(Localized.Yes);
            Enum<Localized>.TryGetValue("OUI", EnumFormat.DisplayName, _french, ignoreCase: true, out value).ShouldBeTrue();
            value.ShouldBe(Localized.Yes);
            Enum<Localized>.TryGetValue("OUI".AsSpan(), EnumFormat.DisplayName, _french, ignoreCase: false, out value).ShouldBeFalse();

            // Getting a converter for another culture must not leave that culture current.
            CultureInfo.CurrentUICulture.ShouldBeSameAs(_english);
        });
    }

    [TestMethod]
    public void LocalizedDisplayNameConvertersCachedByCultureName()
    {
        var converter = EnumConverter<Localized>.GetDefault(EnumFormat.DisplayName, _french);

        EnumConverter<Localized>.GetDefault(EnumFormat.DisplayName, _french).ShouldBeSameAs(converter);
        EnumConverter<Localized>.GetDefault(EnumFormat.DisplayName, new CultureInfo("fr-FR")).ShouldBeSameAs(converter);
        EnumConverter<Localized>.GetDefault(EnumFormat.DisplayName, CultureInfo.ReadOnly(new CultureInfo("fr-FR"))).ShouldBeSameAs(converter);
        EnumConverter<Localized>.GetDefault(EnumFormat.DisplayName, _english).ShouldNotBeSameAs(converter);

        RunWithUICulture(_french, () => EnumConverter<Localized>.GetDefault(EnumFormat.DisplayName).ShouldBeSameAs(converter));
        RunWithUICulture(_english, () => EnumConverter<Localized>.GetDefault(EnumFormat.DisplayName).ShouldNotBeSameAs(converter));
    }

    [TestMethod]
    public void WithDisplayNameGetterStillWorks()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        var converter = new EnumConverter<Status>(opt => opt.WithDisplayNameGetter());
        converter.AsString(Status.Active).ShouldBe("Active (Display)");
        Status.Active.AsString().ShouldBe("Active");
#pragma warning restore CS0618
    }

    private static void RunWithUICulture(CultureInfo culture, Action action)
    {
        var previous = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = culture;

        try
        {
            action();
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    private enum Status
    {
        [Display(Name = "Active (Display)")]
        [Description("Active (Description)")]
        [EnumMember(Value = "active")]
        Active = 1,

        [Display(Name = "Inactive (Display)")]
        [Description("Inactive (Description)")]
        [EnumMember(Value = "inactive")]
        Inactive = 2,

        Unattributed = 3,
    }

    [Flags]
    private enum Options
    {
        [Display(Name = "None (Display)")]
        None = 0,

        [Display(Name = "Alpha (Display)")]
        [Description("Alpha (Description)")]
        [EnumMember(Value = "alpha")]
        Alpha = 1,

        [Display(Name = "Beta (Display)")]
        [Description("Beta (Description)")]
        [EnumMember(Value = "beta")]
        Beta = 2,

        [Display(Name = "Gamma (Display)")]
        [Description("Gamma (Description)")]
        [EnumMember(Value = "gamma")]
        Gamma = 4,

        [Display(Name = "All (Display)")]
        [Description("All (Description)")]
        [EnumMember(Value = "all")]
        All = Alpha | Beta | Gamma,
    }

    private enum Localized
    {
        [Display(Name = nameof(LocalizedResources.Yes), ResourceType = typeof(LocalizedResources))]
        Yes,

        [Display(Name = nameof(LocalizedResources.No), ResourceType = typeof(LocalizedResources))]
        No,

        [Display(Name = "Maybe")]
        Maybe,
    }
}

/// <summary>
/// Stands in for a designer-generated resource class: <see cref="DisplayAttribute"/> only requires public static string properties on the resource type.
/// </summary>
public static class LocalizedResources
{
    public static string Yes => IsFrench ? "Oui" : "Yes";

    public static string No => IsFrench ? "Non" : "No";

    private static bool IsFrench => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr";
}
