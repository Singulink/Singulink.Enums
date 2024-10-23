using System.Diagnostics.CodeAnalysis;

namespace Singulink.Enums;

internal static class EnumRangeInfo<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>
    where T : unmanaged, Enum
{
    public static bool IsContinuous { get; }

    public static T DefinedMin { get; }

    public static T DefinedMax { get; }

    static EnumRangeInfo()
    {
        var values = Enum<T>.GetFields().Select(m => (T)m.GetValue(null)!).Distinct().ToList();

        if (values.Count == 0)
            return;

        DefinedMin = values.Min();
        DefinedMax = values.Max();

        decimal range = Convert.ToDecimal(DefinedMax, null) - Convert.ToDecimal(DefinedMin, null);
        IsContinuous = range == values.Count - 1;
    }
}
