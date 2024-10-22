using System.Runtime.CompilerServices;

namespace Singulink.Enums;

/// <content>
/// Provides HasAnyFlags implementations.
/// </content>
public static partial class EnumExtensions
{
    /// <summary>
    /// Determines whether the value has any of the given flags.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool HasAnyFlag<T>(this T value, T flags) where T : unmanaged, Enum
    {
        if (sizeof(T) == 1)
            return (UnsafeMethods.BitCast<T, byte>(value) & UnsafeMethods.BitCast<T, byte>(flags)) != 0;
        else if (sizeof(T) == 2)
            return (UnsafeMethods.BitCast<T, short>(value) & UnsafeMethods.BitCast<T, short>(flags)) != 0;
        else if (sizeof(T) == 4)
            return (UnsafeMethods.BitCast<T, int>(value) & UnsafeMethods.BitCast<T, int>(flags)) != 0;
        else if (sizeof(T) == 8)
            return (UnsafeMethods.BitCast<T, long>(value) & UnsafeMethods.BitCast<T, long>(flags)) != 0;

        throw new NotSupportedException();
    }

    /// <inheritdoc cref="HasAnyFlag{T}(T, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlag<T>(this T value, T flags1, T flags2) where T : unmanaged, Enum
    {
        return value.HasAnyFlag(flags1.SetFlags(flags2));
    }

    /// <inheritdoc cref="HasAnyFlag{T}(T, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlag<T>(this T value, T flags1, T flags2, T flags3) where T : unmanaged, Enum
    {
        return value.HasAnyFlag(flags1.SetFlags(flags2).SetFlags(flags3));
    }

    /// <inheritdoc cref="HasAnyFlag{T}(T, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlag<T>(this T value, params T[] flags) where T : unmanaged, Enum
    {
        foreach (var flag in flags)
        {
            if (value.HasAnyFlag(flag))
                return true;
        }

        return false;
    }

    /// <inheritdoc cref="HasAnyFlag{T}(T, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyFlag<T>(this T value, IEnumerable<T> flags) where T : unmanaged, Enum
    {
        foreach (var flag in flags)
        {
            if (value.HasAnyFlag(flag))
                return true;
        }

        return false;
    }
}
