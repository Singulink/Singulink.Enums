namespace Singulink.Enums;

/// <summary>
/// Provides options for splitting flags.
/// </summary>
[Flags]
public enum SplitFlagsOptions
{
    /// <summary>
    /// Use the default options when splitting flags.
    /// </summary>
    None = 0,

    /// <summary>
    /// Include all matching flags in the result, even if they are redundant. If this option is not specified, only the minimal set of flags are included.
    /// A zero-value flag is only included if the value has no flags set, regardless of this option.
    /// </summary>
    AllMatchingFlags = 1,

    /// <summary>
    /// Exclude any remainder that cannot be represented by any defined flags from the result. This flag takes precedence over the <see
    /// cref="ThrowOnRemainder"/> flag if both are set.
    /// </summary>
    ExcludeRemainder = 4,

    /// <summary>
    /// Throw <see cref="ArgumentException"/> if there is a remainder that cannot be represented by any defined flags.
    /// </summary>
    ThrowOnRemainder = 8,
}
