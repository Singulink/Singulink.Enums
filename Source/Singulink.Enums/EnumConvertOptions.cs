using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Singulink.Enums;

/// <summary>
/// Provides options for customizing enumeration string conversion behavior.
/// </summary>
public class EnumConvertOptions
{
    private static readonly Func<FieldInfo, string> _displayNameGetter = f => f.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? f.Name;

    private string _separator = ", ";
    internal char _separatorChar = ',';

    /// <summary>
    /// Gets or sets a string containing a separator character with an optional leading and trailing space to be used for flags enumerations. Ignored for
    /// non-flags enumerations. Defaults to <c>", "</c> (comma followed by a space).
    /// </summary>
    /// <remarks>
    /// The separator must contain exactly one separator character with an optional leading and trailing space. The spaces are used for formatting when a value
    /// is converted to a string, but the spaces are optional when parsing a string. The separator character cannot be a hyphen (<c>'-'</c>).
    /// </remarks>
    /// <exception cref="ArgumentException">A hyphen was used as the separator character.</exception>
    public string Separator
    {
        get => _separator;
        set {
            _separatorChar = GetSeparatorChar(value);

            if (_separatorChar is '-')
                throw new ArgumentException("Hyphen is not allowed as a separator character.", nameof(value));

            _separator = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the enumeration parser should ignore case when matching names. Defaults to <see langword="false"/>.
    /// </summary>
    public bool IgnoreCase { get; set; }

    /// <summary>
    /// Gets or sets a function that customizes the names of enumeration values. Defaults to the field name of the enumeration value.
    /// </summary>
    /// <remarks>
    /// The <see cref="WithDisplayNameGetter"/> method can be used to set this property to a function that gets the names of enumeration values from the <see
    /// cref="DisplayAttribute"/> on each value's field.
    /// </remarks>
    public Func<FieldInfo, string> NameGetter { get; set; } = f => f.Name;

    /// <summary>
    /// Sets the <see cref="NameGetter"/> property on this instance to a function that gets the names of enumeration values from the <see
    /// cref="DisplayAttribute"/>'s <see cref="DisplayAttribute.Name"/> property on each enumeration value's field, if present.
    /// </summary>
    /// <returns>The modified options instance (same instance is returned - a new instance is not created).</returns>
    /// <remarks>
    /// If an enumeration value's field does not have a <see cref="DisplayAttribute"/> or the attribute's <see cref="DisplayAttribute.Name"/> property is not
    /// set, the field name of the enumeration value is used instead.
    /// </remarks>
    public EnumConvertOptions WithDisplayNameGetter()
    {
        NameGetter = _displayNameGetter;
        return this;
    }

    private static char GetSeparatorChar(string value)
    {
        if (value.Length == 1)
        {
            return value[0];
        }
        else if (value.Length == 2)
        {
            if (char.IsWhiteSpace(value[0]) && !char.IsWhiteSpace(value[1]))
                return value[1];

            if (!char.IsWhiteSpace(value[0]) && char.IsWhiteSpace(value[1]))
                return value[0];
        }
        else if (value.Length == 3)
        {
            if (char.IsWhiteSpace(value[0]) && !char.IsWhiteSpace(value[1]) && char.IsWhiteSpace(value[2]))
                return value[1];
        }

        throw new ArgumentException(
            "Separator must contain exactly 1 separator character with an optional leading and trailing space.", nameof(value));
    }
}
