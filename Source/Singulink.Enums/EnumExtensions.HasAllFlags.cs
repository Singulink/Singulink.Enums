using System.Runtime.CompilerServices;

namespace Singulink.Enums;

/// <summary>
/// Provides HasAllFlags implementations.
/// </summary>
public static partial class EnumExtensions
{
    /// <summary>
    /// Determines whether the value has all the given flags.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool HasAllFlags<T>(this T value, T flags) where T : unmanaged, Enum
    {
        // #if NET8_0_OR_GREATER
        //        return value.HasFlag(flags);
        // #else

        if (sizeof(T) == 1)
            return (UnsafeMethods.BitCast<T, byte>(value) & UnsafeMethods.BitCast<T, byte>(flags)) == UnsafeMethods.BitCast<T, byte>(flags);
        else if (sizeof(T) == 2)
            return (UnsafeMethods.BitCast<T, short>(value) & UnsafeMethods.BitCast<T, short>(flags)) == UnsafeMethods.BitCast<T, short>(flags);
        else if (sizeof(T) == 4)
            return (UnsafeMethods.BitCast<T, int>(value) & UnsafeMethods.BitCast<T, int>(flags)) == UnsafeMethods.BitCast<T, int>(flags);
        else if (sizeof(T) == 8)
            return (UnsafeMethods.BitCast<T, long>(value) & UnsafeMethods.BitCast<T, long>(flags)) == UnsafeMethods.BitCast<T, long>(flags);

        throw new NotSupportedException();

        // #endif
    }

    /// <inheritdoc cref="HasAllFlags{T}(T, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<T>(this T value, T flags1, T flags2) where T : unmanaged, Enum
    {
        return value.HasAllFlags(flags1.SetFlags(flags2));
    }

    /// <inheritdoc cref="HasAllFlags{T}(T, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAllFlags<T>(this T value, T flags1, T flags2, T flags3) where T : unmanaged, Enum
    {
        return value.HasAllFlags(flags1.SetFlags(flags2).SetFlags(flags3));
    }

    /// <inheritdoc cref="HasAllFlags{T}(T, T)"/>
    public static bool HasAllFlags<T>(this T value, params T[] flags) where T : unmanaged, Enum
    {
        foreach (var flag in flags)
        {
            if (!value.HasAllFlags(flag))
                return false;
        }

        return true;
    }

    /// <inheritdoc cref="HasAllFlags{T}(T, T)"/>
    public static bool HasAllFlags<T>(this T value, params ReadOnlySpan<T> flags) where T : unmanaged, Enum
    {
        foreach (var flag in flags)
        {
            if (!value.HasAllFlags(flag))
                return false;
        }

        return true;
    }

    /// <inheritdoc cref="HasAllFlags{T}(T, T)"/>
    public static bool HasAllFlags<T>(this T value, IEnumerable<T> flags) where T : unmanaged, Enum
    {
        foreach (var flag in flags)
        {
            if (!value.HasAllFlags(flag))
                return false;
        }

        return true;
    }
}
