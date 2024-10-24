using System.Runtime.CompilerServices;

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
    /// Zero-value flags are never included in a <see cref="EnumExtensions.SplitFlags{T}(T, SplitFlagsOptions)"/> result regardless of this option.
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

#pragma warning disable SA1649 // File name should match first type name

internal static class SplitFlagsOptionsExtensions
{
    public static void EnsureValid(this SplitFlagsOptions options, string paramName)
    {
        if (!options.AreFlagsDefined())
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            static void Throw(string paramName) => throw new ArgumentException("Split flag options are invalid.", paramName);
            Throw(paramName);
        }
    }

    public static void ThrowIfThrowOnRemainderSet(this SplitFlagsOptions options, string valueParamName)
    {
        if (options.HasAllFlags(SplitFlagsOptions.ThrowOnRemainder))
        {
            static void Throw(string valueParamName) => throw new ArgumentException("The value contains a remainder with undefined flags.", nameof(valueParamName));
            Throw(valueParamName);
        }
    }
}
