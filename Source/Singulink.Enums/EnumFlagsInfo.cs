using System.Diagnostics.CodeAnalysis;

namespace Singulink.Enums;

internal static class EnumFlagsInfo<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>
    where T : unmanaged, Enum
{
    internal static readonly T AllDefinedFlags = default(T).SetFlags(Enum<T>.Values);

    internal static readonly IReadOnlyList<T> DefaultValueOnlyList = [default];
}
