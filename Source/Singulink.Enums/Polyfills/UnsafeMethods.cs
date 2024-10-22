namespace System.Runtime.CompilerServices;

#pragma warning disable CS8500 // address / sizeof of managed types

/// <summary>
/// Polyfilled Unsafe methods for .NET Standard compatibility.
/// </summary>
internal static class UnsafeMethods
{
#if !NETSTANDARD
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TTo BitCast<TFrom, TTo>(TFrom source)
        where TFrom : struct
        where TTo : struct
    {
        return Unsafe.BitCast<TFrom, TTo>(source);
    }
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe TTo BitCast<TFrom, TTo>(TFrom source)
        where TFrom : struct
        where TTo : struct
    {
        if (sizeof(TFrom) != sizeof(TTo))
        {
            static void Throw() => throw new NotSupportedException();
            Throw();
        }

        return Unsafe.ReadUnaligned<TTo>(ref Unsafe.As<TFrom, byte>(ref source));
    }
#endif
}
