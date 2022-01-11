using System;
using System.Linq;

namespace Singulink.Enums
{
    internal static class EnumFlagsInfo<T> where T : unmanaged, Enum
    {
        public static T AllDefinedFlags { get; } = default(T).SetFlags(Enum<T>.GetMembers().Select(m => m.Value));
    }
}