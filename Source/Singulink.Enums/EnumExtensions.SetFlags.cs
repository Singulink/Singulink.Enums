using System.Runtime.CompilerServices;

namespace Singulink.Enums;

/// <content>
/// Provides SetFlags implementations.
/// </content>
public static partial class EnumExtensions
{
    /// <summary>
    /// Sets the specified flags on the value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe T SetFlags<T>(this T value, T flags) where T : unmanaged, Enum
    {
        if (sizeof(T) == 1)
            return UnsafeMethods.BitCast<byte, T>((byte)(UnsafeMethods.BitCast<T, byte>(value) | UnsafeMethods.BitCast<T, byte>(flags)));
        else if (sizeof(T) == 2)
            return UnsafeMethods.BitCast<short, T>((short)(UnsafeMethods.BitCast<T, short>(value) | UnsafeMethods.BitCast<T, short>(flags)));
        else if (sizeof(T) == 4)
            return UnsafeMethods.BitCast<int, T>(UnsafeMethods.BitCast<T, int>(value) | UnsafeMethods.BitCast<T, int>(flags));
        else if (sizeof(T) == 8)
            return UnsafeMethods.BitCast<long, T>(UnsafeMethods.BitCast<T, long>(value) | UnsafeMethods.BitCast<T, long>(flags));

        throw new NotSupportedException();
    }

    /// <inheritdoc cref="SetFlags{T}(T, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T SetFlags<T>(this T value, T flags1, T flags2) where T : unmanaged, Enum
    {
        return value.SetFlags(flags1).SetFlags(flags2);
    }

    /// <inheritdoc cref="SetFlags{T}(T, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T SetFlags<T>(this T value, T flags1, T flags2, T flags3) where T : unmanaged, Enum
    {
        return value.SetFlags(flags1).SetFlags(flags2).SetFlags(flags3);
    }

    /// <inheritdoc cref="SetFlags{T}(T, T)"/>
    public static T SetFlags<T>(this T value, params T[] flags) where T : unmanaged, Enum
    {
        foreach (var flag in flags)
            value = value.SetFlags(flag);

        return value;
    }

    /// <inheritdoc cref="SetFlags{T}(T, T)"/>
    public static T SetFlags<T>(this T value, params ReadOnlySpan<T> flags) where T : unmanaged, Enum
    {
        foreach (var flag in flags)
            value = value.SetFlags(flag);

        return value;
    }

    /// <inheritdoc cref="SetFlags{T}(T, T)"/>
    public static T SetFlags<T>(this T value, IEnumerable<T> flags) where T : unmanaged, Enum
    {
        foreach (var flag in flags)
            value = value.SetFlags(flag);

        return value;
    }
}
