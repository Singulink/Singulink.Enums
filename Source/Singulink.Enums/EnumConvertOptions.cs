using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;

namespace Singulink.Enums;

/// <summary>
/// Provides options for customizing enumeration string conversion behavior.
/// </summary>
public class EnumConvertOptions
{
    private static readonly Func<FieldInfo, string> _nameGetter = f => f.Name;
    private static readonly Func<FieldInfo, string> _displayNameGetter = f => f.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? f.Name;
    private static readonly Func<FieldInfo, string> _descriptionGetter = f => f.GetCustomAttribute<DescriptionAttribute>()?.Description ?? f.Name;
    private static readonly Func<FieldInfo, string> _enumMemberValueGetter = f => f.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? f.Name;

    private string _separator = ", ";
    internal char _separatorChar = ',';

    /// <summary>
    /// Gets or sets a string containing a separator character with optional leading and trailing whitespace to be used for flags enumerations. Ignored for
    /// non-flags enumerations. Defaults to <c>", "</c> (comma followed by a space).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The separator must contain a separator character with optional leading and trailing whitespace. The whitespace is used for formatting when a value is
    /// converted to a string, but any whitespace is optional when parsing a string. The separator character cannot be a hyphen (<c>'-'</c>).</para>
    /// <para>
    /// If the separator string contains only whitespace, then it must either consist of a single distinct whitespace character (repeated as many times as
    /// desired for formatting), or it must contain a newline character, in which case the newline character will be used as the parsing separator character
    /// regardless of what other whitespace it contains.</para>
    /// </remarks>
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
    /// The <see cref="WithFormat(EnumFormat)"/> method can be used to set this property to a function that gets the names of enumeration values from one of
    /// the standard sources described by <see cref="EnumFormat"/>.
    /// </remarks>
    public Func<FieldInfo, string> NameGetter { get; set; } = _nameGetter;

    /// <summary>
    /// Sets the <see cref="NameGetter"/> property on this instance to a function that gets the names of enumeration values from the source described by the
    /// specified format.
    /// </summary>
    /// <param name="format">The source of the enumeration names.</param>
    /// <returns>The modified options instance (same instance is returned - a new instance is not created).</returns>
    /// <remarks>
    /// Names obtained from the <see cref="EnumFormat.DisplayName"/> format are resolved when the converter is created, so a converter created with this format
    /// for an enumeration that uses localized display names is specific to the <see cref="System.Globalization.CultureInfo.CurrentUICulture"/> that was
    /// current when it was created. Use <see cref="EnumConverter{T}.GetDefault(EnumFormat, System.Globalization.CultureInfo, bool)"/> to get shared
    /// converters for specific cultures.
    /// </remarks>
    public EnumConvertOptions WithFormat(EnumFormat format)
    {
        NameGetter = GetNameGetter(format);
        return this;
    }

    /// <summary>
    /// Sets the <see cref="NameGetter"/> property on this instance to a function that gets the names of enumeration values from the <see
    /// cref="DisplayAttribute"/>'s <see cref="DisplayAttribute.Name"/> property on each enumeration value's field, if present.
    /// </summary>
    /// <returns>The modified options instance (same instance is returned - a new instance is not created).</returns>
    [Obsolete("Use WithFormat(EnumFormat.DisplayName) instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public EnumConvertOptions WithDisplayNameGetter() => WithFormat(EnumFormat.DisplayName);

    internal static Func<FieldInfo, string> GetNameGetter(EnumFormat format)
    {
        return format switch
        {
            EnumFormat.Name => _nameGetter,
            EnumFormat.DisplayName => _displayNameGetter,
            EnumFormat.Description => _descriptionGetter,
            EnumFormat.EnumMemberValue => _enumMemberValueGetter,
            _ => throw new ArgumentOutOfRangeException(nameof(format), "Unsupported enumeration format."),
        };
    }

    private static char GetSeparatorChar(string value)
    {
        if (value.Length is 1)
            return value[0];

        var trimmed = value.AsSpan().Trim();

        if (trimmed.Length is 1)
            return trimmed[0];

        if (trimmed.Length is 0)
        {
            if (value.Contains('\n'))
                return '\n';

            char[] distinct = [.. value.Distinct()];

            if (distinct.Length is 1)
                return distinct[0];
        }

        throw new ArgumentException("Separator must contain a separator character with optional leading and trailing whitespace.", nameof(value));
    }
}
