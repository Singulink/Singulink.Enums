using System.Globalization;
using System.Runtime.CompilerServices;

namespace Singulink.Enums;

internal static unsafe class UnderlyingOperations
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse<T>(string s, out T result) where T : unmanaged, Enum
    {
        const NumberStyles style = NumberStyles.AllowLeadingSign;
        var culture = CultureInfo.InvariantCulture;
        var underlying = typeof(T).GetEnumUnderlyingType();

        if (sizeof(T) is 1)
        {
            if (underlying == typeof(byte))
                return byte.TryParse(s, style, culture, out byte r) & ToResult(r, out result);

            if (underlying == typeof(sbyte))
                return sbyte.TryParse(s, style, culture, out sbyte r) & ToResult(r, out result);
        }
        else if (sizeof(T) is 2)
        {
            if (underlying == typeof(short))
                return short.TryParse(s, style, culture, out short r) & ToResult(r, out result);

            if (underlying == typeof(ushort))
                return ushort.TryParse(s, style, culture, out ushort r) & ToResult(r, out result);

            if (underlying == typeof(char))
                return char.TryParse(s, out char r) & ToResult(r, out result);
        }
        else if (sizeof(T) is 4)
        {
            if (underlying == typeof(int))
                return int.TryParse(s, style, culture, out int r) & ToResult(r, out result);

            if (underlying == typeof(uint))
                return uint.TryParse(s, style, culture, out uint r) & ToResult(r, out result);
        }
        else if (sizeof(T) is 8)
        {
            if (underlying == typeof(long))
                return long.TryParse(s, style, culture, out long r) & ToResult(r, out result);

            if (underlying == typeof(ulong))
                return ulong.TryParse(s, style, culture, out ulong r) & ToResult(r, out result);
        }

        throw new NotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool ToResult<TValue>(TValue value, out T result) where TValue : struct
        {
            result = UnsafeMethods.BitCast<TValue, T>(value);
            return true;
        }
    }

#if NET9_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse<T>(ReadOnlySpan<char> s, out T result) where T : unmanaged, Enum
    {
        const NumberStyles style = NumberStyles.AllowLeadingSign;
        var culture = CultureInfo.InvariantCulture;
        var underlying = typeof(T).GetEnumUnderlyingType();

        if (sizeof(T) is 1)
        {
            if (underlying == typeof(byte))
                return byte.TryParse(s, style, culture, out byte r) & ToResult(r, out result);

            if (underlying == typeof(sbyte))
                return sbyte.TryParse(s, style, culture, out sbyte r) & ToResult(r, out result);
        }
        else if (sizeof(T) is 2)
        {
            if (underlying == typeof(short))
                return short.TryParse(s, style, culture, out short r) & ToResult(r, out result);

            if (underlying == typeof(ushort))
                return ushort.TryParse(s, style, culture, out ushort r) & ToResult(r, out result);

            if (underlying == typeof(char))
            {
                if (s is [var c])
                {
                    ToResult(c, out result);
                    return true;
                }
                else
                {
                    result = default;
                    return false;
                }
            }
        }
        else if (sizeof(T) is 4)
        {
            if (underlying == typeof(int))
                return int.TryParse(s, style, culture, out int r) & ToResult(r, out result);

            if (underlying == typeof(uint))
                return uint.TryParse(s, style, culture, out uint r) & ToResult(r, out result);
        }
        else if (sizeof(T) is 8)
        {
            if (underlying == typeof(long))
                return long.TryParse(s, style, culture, out long r) & ToResult(r, out result);

            if (underlying == typeof(ulong))
                return ulong.TryParse(s, style, culture, out ulong r) & ToResult(r, out result);
        }

        throw new NotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool ToResult<TValue>(TValue value, out T result) where TValue : struct
        {
            result = UnsafeMethods.BitCast<TValue, T>(value);
            return true;
        }
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString<T>(T value) where T : unmanaged, Enum
    {
        var culture = CultureInfo.InvariantCulture;
        var underlyingType = typeof(T).GetEnumUnderlyingType();

        if (sizeof(T) is 1)
        {
            if (underlyingType == typeof(byte))
                return UnsafeMethods.BitCast<T, byte>(value).ToString(culture);

            if (underlyingType == typeof(sbyte))
                return UnsafeMethods.BitCast<T, sbyte>(value).ToString(culture);
        }
        else if (sizeof(T) is 2)
        {
            if (underlyingType == typeof(short))
                return UnsafeMethods.BitCast<T, short>(value).ToString(culture);

            if (underlyingType == typeof(ushort))
                return UnsafeMethods.BitCast<T, ushort>(value).ToString(culture);

            if (underlyingType == typeof(char))
                return UnsafeMethods.BitCast<T, char>(value).ToString();
        }
        else if (sizeof(T) is 4)
        {
            if (underlyingType == typeof(int))
                return UnsafeMethods.BitCast<T, int>(value).ToString(culture);

            if (underlyingType == typeof(uint))
                return UnsafeMethods.BitCast<T, uint>(value).ToString(culture);
        }
        else if (sizeof(T) is 8)
        {
            if (underlyingType == typeof(long))
                return UnsafeMethods.BitCast<T, long>(value).ToString(culture);

            if (underlyingType == typeof(ulong))
                return UnsafeMethods.BitCast<T, ulong>(value).ToString(culture);
        }

        throw new NotSupportedException();
    }
}
