using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Singulink.Enums;

/// <content>
/// Provides SplitFlags implementations.
/// </content>
public static partial class EnumExtensions
{
    /// <summary>
    /// Splits the value into the defined flags that make up the value, plus any remainder (if <see cref="SplitFlagsOptions.ExcludeRemainder"/> is not set).
    /// </summary>
    /// <param name="value">The value to split.</param>
    /// <param name="options">The options to use for the splitting operation.</param>
    /// <remarks>
    /// If <see cref="SplitFlagsOptions.ExcludeRemainder"/> is not set and there is a remainder that cannot be represented by any defined flags then its value
    /// is appended to the end of the resulting list. You can check if the last element is a remainder by calling <see cref="EnumExtensions.IsDefined{T}(T)"/>
    /// on it. You can also exclude the remainder or throw an exception if there is a remainder via the <paramref name="options"/> parameter.
    /// </remarks>
    [SkipLocalsInit]
    public static IReadOnlyList<T> SplitFlags<[DynamicallyAccessedMembers(PublicFields)] T>(this T value, SplitFlagsOptions options = SplitFlagsOptions.None)
        where T : unmanaged, Enum
    {
        Enum<T>.EnsureIsFlagsEnum();
        options.EnsureValid(nameof(options));

        if (EqualityComparer<T>.Default.Equals(value, default))
            return [];

        bool allMatchingFlags = options.HasAllFlags(SplitFlagsOptions.AllMatchingFlags);

        // If AllMatchingFlags is not specified then max possible number of matched flags is 64, otherwise it's the number of defined flags.
        // AllMatching flags will be rare and even more rare with > 64 flags so we stack alloc for the common case.

        const int MaxStackAllocLength = 64;
        bool doStackAlloc = Enum<T>.Values.Length <= MaxStackAllocLength || !allMatchingFlags;
        int[] rented = null;
        Span<int> foundItems = doStackAlloc ? stackalloc int[MaxStackAllocLength] : (rented = ArrayPool<int>.Shared.Rent(Enum<T>.Values.Length));

        SplitFlagsDescending(value, allMatchingFlags, foundItems, out int foundItemsCount, out T remainder);

        bool skipRemainder = EqualityComparer<T>.Default.Equals(remainder, default) || options.HasAllFlags(SplitFlagsOptions.ExcludeRemainder);
        T[] results;

        if (skipRemainder)
        {
            results = new T[foundItemsCount];
        }
        else if (!options.HasAllFlags(SplitFlagsOptions.ThrowOnRemainder))
        {
            results = new T[foundItemsCount + 1];
            results[foundItemsCount] = remainder;
        }
        else
        {
            throw new ArgumentException("The value contains undefined flags.", nameof(value));
        }

        for (int i = 0; i < foundItemsCount; i++)
        {
            results[foundItemsCount - 1 - i] = Enum<T>.Values[foundItems[i]];
        }

        if (rented is not null)
            ArrayPool<int>.Shared.Return(rented);

        return results;
    }

    internal static void SplitFlagsDescending<[DynamicallyAccessedMembers(PublicFields)] T>(
        T value,
        bool allMatchingFlags,
        Span<int> foundItems,
        out int foundItemsCount,
        out T remainder)
        where T : unmanaged, Enum
    {
        foundItemsCount = 0;
        remainder = value;

        if (allMatchingFlags)
        {
            for (int i = Enum<T>.Values.Length - 1; i >= 0; i--)
            {
                var definedValue = Enum<T>.Values[i];

                if (EqualityComparer<T>.Default.Equals(definedValue, default))
                    continue;

                if (value.HasAllFlags(definedValue))
                {
                    foundItems[foundItemsCount++] = i;
                    remainder = remainder.ClearFlags(definedValue);
                }
            }
        }
        else
        {
            for (int i = Enum<T>.Values.Length - 1; i >= 0; i--)
            {
                var definedValue = Enum<T>.Values[i];

                if (EqualityComparer<T>.Default.Equals(definedValue, default))
                    continue;

                if (remainder.HasAllFlags(definedValue))
                {
                    foundItems[foundItemsCount++] = i;
                    remainder = remainder.ClearFlags(definedValue);

                    if (EqualityComparer<T>.Default.Equals(remainder, default))
                        break;
                }
            }
        }
    }
}
