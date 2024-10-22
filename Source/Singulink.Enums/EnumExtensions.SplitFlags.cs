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
    /// Splits the value into the defined flags that make up the value, plus the remainder (if <see cref="SplitFlagsOptions.ExcludeRemainder"/> is not set).
    /// </summary>
    /// <param name="value">The value to split.</param>
    /// <param name="options">The options to use during for the splitting operation.</param>
    [SkipLocalsInit]
    public static IReadOnlyList<T> SplitFlags<[DynamicallyAccessedMembers(PublicFields)] T>(this T value, SplitFlagsOptions options = SplitFlagsOptions.None)
        where T : unmanaged, Enum
    {
        if (!Enum<T>.IsFlagsEnum)
            throw new InvalidOperationException($"Type '{typeof(T)}' is not a flags enumeration.");

        if (!options.IsValid())
            throw new ArgumentException("Value of options flags is invalid.", nameof(options));

        if (EqualityComparer<T>.Default.Equals(value, default))
        {
            if (default(T).IsDefined())
                return EnumFlagsInfo<T>.DefaultValueOnlyList;

            return [];
        }

        bool doStackAlloc = Enum<T>.Values.Length <= 79 || !options.HasAllFlags(SplitFlagsOptions.AllMatchingFlags);

        int[] rented = null;

        // Needs one extra slot for possible remainder
        Span<int> foundItems = doStackAlloc ? stackalloc int[80] : (rented = ArrayPool<int>.Shared.Rent(Enum<T>.Values.Length + 1));
        SplitFlagsDescending(value, options, foundItems, out int foundItemsCount, out T remainder);

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
        SplitFlagsOptions options,
        Span<int> foundItems,
        out int foundItemsCount,
        out T remainder)
        where T : unmanaged, Enum
    {
        foundItemsCount = 0;
        remainder = value;

        if (options.HasAllFlags(SplitFlagsOptions.AllMatchingFlags))
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
