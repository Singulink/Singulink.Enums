using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Singulink.Enums;

internal static class EnumFlagsInfo<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>
    where T : unmanaged, Enum
{
    internal static readonly T AllFlags = default(T).SetFlags(Enum<T>.Values);

    internal static readonly T AllSingleBitFlags = default(T).SetFlags(Enum<T>.Values.Where(v => v.HasSingleBitSet()));

    internal static readonly bool AreAllFlagsDefinedBySingleBits = EqualityComparer<T>.Default.Equals(AllFlags, AllSingleBitFlags);

    internal static readonly ImmutableArray<T> MultiBitValuesDescending = Enum<T>.Values.Reverse().Where(v => !v.HasSingleBitSet()).ToImmutableArray();

    internal static readonly bool HasSingleMultiBitValue = MultiBitValuesDescending.Length is 1;

    internal static readonly T SingleMultiBitValue = HasSingleMultiBitValue ? MultiBitValuesDescending[0] : default;
}
