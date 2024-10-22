using System.Buffers;

namespace System;

/// <summary>
/// Polyfilled string methods for .NET Standard compatibility.
/// </summary>
internal static class StringMethods
{
#if !NETSTANDARD

    public static string Create<TState>(int length, TState state, SpanAction<char, TState> action) => string.Create(length, state, action);

#else

    public static string Create<TState>(int length, TState state, SpanAction<char, TState> action)
    {
        if (length <= 0)
        {
            if (length == 0)
                return string.Empty;

            throw new ArgumentOutOfRangeException(nameof(length));
        }

        char[] rented = null;

        Span<char> buffer = length <= 256 ? stackalloc char[256] : (rented = ArrayPool<char>.Shared.Rent(length));
        buffer = buffer[..length];

        action(buffer, state);
        string result = buffer.ToString();

        if (rented is not null)
            ArrayPool<char>.Shared.Return(rented);

        return result;
    }

#endif
}
