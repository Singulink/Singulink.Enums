using System;
using System.Linq;

namespace Singulink.Enums
{
    internal static class EnumRangeInfo<T> where T : unmanaged, Enum
    {
        public static bool IsContinuous { get; }

        public static T DefinedMin { get; }

        public static T DefinedMax { get; }

        static EnumRangeInfo()
        {
            var values = Enum<T>.GetMembers().Select(m => m.Value).Distinct().ToArray();

            if (values.Length == 0)
                return;

            DefinedMin = values.Min();
            DefinedMax = values.Max();

            decimal range = Convert.ToDecimal(DefinedMax, null) - Convert.ToDecimal(DefinedMin, null);
            IsContinuous = range == values.Length - 1;
        }
    }
}