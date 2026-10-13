using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.Serialization;

namespace Singulink.Enums;

/// <summary>
/// Specifies the source of the names used when converting enumeration values to and from strings.
/// </summary>
public enum EnumFormat
{
    /// <summary>
    /// Enumeration values are represented by their field names.
    /// </summary>
    Name = 0,

    /// <summary>
    /// Enumeration values are represented by the <see cref="DisplayAttribute.Name"/> property of the <see cref="DisplayAttribute"/> applied to their field,
    /// or by their field name if the attribute is not present or the property is not set. Names are localized using
    /// <see cref="CultureInfo.CurrentUICulture"/> if the attribute specifies a <see cref="DisplayAttribute.ResourceType"/>.
    /// </summary>
    DisplayName = 1,

    /// <summary>
    /// Enumeration values are represented by the <see cref="DescriptionAttribute.Description"/> property of the <see cref="DescriptionAttribute"/> applied to
    /// their field, or by their field name if the attribute is not present or the property is not set.
    /// </summary>
    Description = 2,

    /// <summary>
    /// Enumeration values are represented by the <see cref="EnumMemberAttribute.Value"/> property of the <see cref="EnumMemberAttribute"/> applied to their
    /// field, or by their field name if the attribute is not present or the property is not set.
    /// </summary>
    EnumMemberValue = 3,
}
