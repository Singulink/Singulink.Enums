using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Singulink.Enums.Tests;

[PrefixTestClass]
public class EnumAttributeTests
{
    [TestMethod]
    public void GetAttribute()
    {
        var display = Enum<Status>.GetAttribute<DisplayAttribute>(Status.Active);
        display.ShouldNotBeNull();
        display.Name.ShouldBe("Active (Display)");
        display.GetDescription().ShouldBe("Currently active.");

        Enum<Status>.GetAttribute<DescriptionAttribute>(Status.Active).ShouldNotBeNull().Description.ShouldBe("Active (Description)");
    }

    [TestMethod]
    public void GetAttributeMissing()
    {
        Enum<Status>.GetAttribute<DisplayAttribute>(Status.Unattributed).ShouldBeNull();
        Enum<Status>.GetAttribute<DescriptionAttribute>(Status.Inactive).ShouldBeNull();
    }

    [TestMethod]
    public void GetAttributeUndefinedValue()
    {
        Enum<Status>.GetAttribute<DisplayAttribute>((Status)42).ShouldBeNull();
    }

    [TestMethod]
    public void GetAttributeBaseType()
    {
        Enum<Status>.GetAttribute<Attribute>(Status.Active).ShouldNotBeNull();
        Enum<Status>.GetAttribute<Attribute>(Status.Unattributed).ShouldBeNull();
    }

    [TestMethod]
    public void GetAttributeFlags()
    {
        Enum<Options>.GetAttribute<DisplayAttribute>(Options.Alpha).ShouldNotBeNull().Name.ShouldBe("Alpha (Display)");
        Enum<Options>.GetAttribute<DisplayAttribute>(Options.All).ShouldNotBeNull().Name.ShouldBe("All (Display)");

        // Combinations that are not themselves defined have no field and therefore no attributes.
        Enum<Options>.GetAttribute<DisplayAttribute>(Options.Alpha | Options.Gamma).ShouldBeNull();
    }

    [TestMethod]
    public void GetAttributeDuplicateValuesUsesFirstField()
    {
        Enum<Duplicates>.GetAttribute<DisplayAttribute>(Duplicates.First).ShouldNotBeNull().Name.ShouldBe("First");
        Enum<Duplicates>.GetAttribute<DisplayAttribute>(Duplicates.Second).ShouldNotBeNull().Name.ShouldBe("First");
        Enum<Duplicates>.GetAttribute<DisplayAttribute>(Duplicates.Third).ShouldNotBeNull().Name.ShouldBe("Third");
        Enum<Duplicates>.GetAttribute<DisplayAttribute>(Duplicates.Fourth).ShouldNotBeNull().Name.ShouldBe("Third");
    }

    [TestMethod]
    public void GetAttributeIsCached()
    {
        var first = Enum<Status>.GetAttribute<DisplayAttribute>(Status.Active);
        var second = Enum<Status>.GetAttribute<DisplayAttribute>(Status.Active);
        first.ShouldBeSameAs(second);
    }

    [TestMethod]
    public void GetAttributeLocalizedDescriptionResolvesAtAccessTime()
    {
        var attribute = Enum<Localized>.GetAttribute<DisplayAttribute>(Localized.Yes).ShouldNotBeNull();
        var previous = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            attribute.GetDescription().ShouldBe("Yes");

            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");
            attribute.GetDescription().ShouldBe("Oui");
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    private enum Status
    {
        [Display(Name = "Active (Display)", Description = "Currently active.")]
        [Description("Active (Description)")]
        Active = 1,

        [Display(Name = "Inactive (Display)")]
        Inactive = 2,

        Unattributed = 3,
    }

    [Flags]
    private enum Options
    {
        None = 0,

        [Display(Name = "Alpha (Display)")]
        Alpha = 1,

        [Display(Name = "Beta (Display)")]
        Beta = 2,

        [Display(Name = "Gamma (Display)")]
        Gamma = 4,

        [Display(Name = "All (Display)")]
        All = Alpha | Beta | Gamma,
    }

    private enum Duplicates
    {
        [Display(Name = "First")]
        First = 1,

        [Display(Name = "Second")]
        Second = 1,

        [Display(Name = "Third")]
        Third = 2,

        [Display(Name = "Fourth")]
        Fourth = 2,
    }

    private enum Localized
    {
        [Display(Description = nameof(LocalizedResources.Yes), ResourceType = typeof(LocalizedResources))]
        Yes,
    }
}
