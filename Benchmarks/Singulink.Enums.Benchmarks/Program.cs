using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

#pragma warning disable SA1005
#pragma warning disable SA1515

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class Program
{
    private readonly AttributeTargets[] _allValues = Enum.GetValues<AttributeTargets>();
    private readonly AttributeTargets _singleAllFlagsValue = AttributeTargets.All;
    private readonly AttributeTargets _manySingleFlags = AttributeTargets.All & ~AttributeTargets.Assembly;

    private bool _boolResult;
    private AttributeTargets _enumResult;

    [BenchmarkCategory("HasAllFlags"), Benchmark(Baseline = true, OperationsPerInvoke = 10)]
    public void HasAllFlags_Singulink()
    {
        var value = _singleAllFlagsValue;
        _boolResult = Singulink.Enums.EnumExtensions.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAllFlags(value, AttributeTargets.Assembly);
    }

    [BenchmarkCategory("HasAllFlags"), Benchmark(OperationsPerInvoke = 10)]
    public void HasAllFlags_EnumsNet()
    {
        var value = _singleAllFlagsValue;
        _boolResult = EnumsNET.FlagEnums.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAllFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAllFlags(value, AttributeTargets.Assembly);
    }

    [BenchmarkCategory("HasAllFlags"), Benchmark(OperationsPerInvoke = 10)]
    public void HasAllFlags_System()
    {
        var value = _singleAllFlagsValue;
        _boolResult = value.HasFlag(AttributeTargets.Assembly);
        _boolResult = value.HasFlag(AttributeTargets.Assembly);
        _boolResult = value.HasFlag(AttributeTargets.Assembly);
        _boolResult = value.HasFlag(AttributeTargets.Assembly);
        _boolResult = value.HasFlag(AttributeTargets.Assembly);
        _boolResult = value.HasFlag(AttributeTargets.Assembly);
        _boolResult = value.HasFlag(AttributeTargets.Assembly);
        _boolResult = value.HasFlag(AttributeTargets.Assembly);
        _boolResult = value.HasFlag(AttributeTargets.Assembly);
        _boolResult = value.HasFlag(AttributeTargets.Assembly);
    }

    [BenchmarkCategory("HasAnyFlags"), Benchmark(Baseline = true, OperationsPerInvoke = 10)]
    public void HasAnyFlags_Singulink()
    {
        var value = _singleAllFlagsValue;
        _boolResult = Singulink.Enums.EnumExtensions.HasAnyFlag(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAnyFlag(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAnyFlag(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAnyFlag(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAnyFlag(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAnyFlag(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAnyFlag(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAnyFlag(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAnyFlag(value, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAnyFlag(value, AttributeTargets.Assembly);
    }

    [BenchmarkCategory("HasAnyFlags"), Benchmark(OperationsPerInvoke = 10)]
    public void HasAnyFlags_EnumsNet()
    {
        var value = _singleAllFlagsValue;
        _boolResult = EnumsNET.FlagEnums.HasAnyFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAnyFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAnyFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAnyFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAnyFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAnyFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAnyFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAnyFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAnyFlags(value, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAnyFlags(value, AttributeTargets.Assembly);
    }

    [BenchmarkCategory("HasAnyFlags"), Benchmark(OperationsPerInvoke = 10)]
    public void HasAnyFlags_Operator()
    {
        var value = _singleAllFlagsValue;
        _boolResult = (value & AttributeTargets.Assembly) != 0;
        _boolResult = (value & AttributeTargets.Assembly) != 0;
        _boolResult = (value & AttributeTargets.Assembly) != 0;
        _boolResult = (value & AttributeTargets.Assembly) != 0;
        _boolResult = (value & AttributeTargets.Assembly) != 0;
        _boolResult = (value & AttributeTargets.Assembly) != 0;
        _boolResult = (value & AttributeTargets.Assembly) != 0;
        _boolResult = (value & AttributeTargets.Assembly) != 0;
        _boolResult = (value & AttributeTargets.Assembly) != 0;
        _boolResult = (value & AttributeTargets.Assembly) != 0;
    }

    [BenchmarkCategory("SetFlags"), Benchmark(Baseline = true, OperationsPerInvoke = 10)]
    public void SetFlags_Singulink()
    {
        var value = _singleAllFlagsValue;
        _enumResult = Singulink.Enums.EnumExtensions.SetFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.SetFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.SetFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.SetFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.SetFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.SetFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.SetFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.SetFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.SetFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.SetFlags(value, AttributeTargets.Assembly);
    }

    [BenchmarkCategory("SetFlags"), Benchmark(OperationsPerInvoke = 10)]
    public void SetFlags_EnumsNet()
    {
        var value = _singleAllFlagsValue;
        _enumResult = EnumsNET.FlagEnums.CombineFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.CombineFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.CombineFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.CombineFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.CombineFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.CombineFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.CombineFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.CombineFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.CombineFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.CombineFlags(value, AttributeTargets.Assembly);
    }

    [BenchmarkCategory("SetFlags"), Benchmark(OperationsPerInvoke = 10)]
    public void SetFlags_Operator()
    {
        var value = _singleAllFlagsValue;
        _enumResult = value | AttributeTargets.Assembly;
        _enumResult = value | AttributeTargets.Assembly;
        _enumResult = value | AttributeTargets.Assembly;
        _enumResult = value | AttributeTargets.Assembly;
        _enumResult = value | AttributeTargets.Assembly;
        _enumResult = value | AttributeTargets.Assembly;
        _enumResult = value | AttributeTargets.Assembly;
        _enumResult = value | AttributeTargets.Assembly;
        _enumResult = value | AttributeTargets.Assembly;
        _enumResult = value | AttributeTargets.Assembly;
    }

    [BenchmarkCategory("ClearFlags"), Benchmark(Baseline = true, OperationsPerInvoke = 10)]
    public void ClearFlags_Singulink()
    {
        var value = _singleAllFlagsValue;
        _enumResult = Singulink.Enums.EnumExtensions.ClearFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.ClearFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.ClearFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.ClearFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.ClearFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.ClearFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.ClearFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.ClearFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.ClearFlags(value, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.ClearFlags(value, AttributeTargets.Assembly);
    }

    [BenchmarkCategory("ClearFlags"), Benchmark(OperationsPerInvoke = 10)]
    public void ClearFlags_EnumsNet()
    {
        var value = _singleAllFlagsValue;
        _enumResult = EnumsNET.FlagEnums.RemoveFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.RemoveFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.RemoveFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.RemoveFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.RemoveFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.RemoveFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.RemoveFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.RemoveFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.RemoveFlags(value, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.RemoveFlags(value, AttributeTargets.Assembly);
    }

    [BenchmarkCategory("ClearFlags"), Benchmark(OperationsPerInvoke = 10)]
    public void ClearFlags_Operator()
    {
        var value = _singleAllFlagsValue;
        _enumResult = value & ~AttributeTargets.Assembly;
        _enumResult = value & ~AttributeTargets.Assembly;
        _enumResult = value & ~AttributeTargets.Assembly;
        _enumResult = value & ~AttributeTargets.Assembly;
        _enumResult = value & ~AttributeTargets.Assembly;
        _enumResult = value & ~AttributeTargets.Assembly;
        _enumResult = value & ~AttributeTargets.Assembly;
        _enumResult = value & ~AttributeTargets.Assembly;
        _enumResult = value & ~AttributeTargets.Assembly;
        _enumResult = value & ~AttributeTargets.Assembly;
    }

    [BenchmarkCategory("IsDefined"), Benchmark(Baseline = true, OperationsPerInvoke = 16)]
    public void IsDefined_Singulink()
    {
        foreach (var v in _allValues)
            _boolResult = Singulink.Enums.EnumExtensions.IsDefined(v);
    }

    [BenchmarkCategory("IsDefined"), Benchmark(OperationsPerInvoke = 16)]
    public void IsDefined_EnumsNet()
    {
        foreach (var v in _allValues)
            _boolResult = EnumsNET.Enums.IsDefined(v);
    }

    [BenchmarkCategory("IsDefined"), Benchmark(OperationsPerInvoke = 16)]
    public void IsDefined_System()
    {
        foreach (var v in _allValues)
            _boolResult = Enum.IsDefined(v);
    }

    [BenchmarkCategory("AreFlagsDefined"), Benchmark(Baseline = true, OperationsPerInvoke = 10)]
    public void AreFlagsDefined_Singulink()
    {
        var value = _singleAllFlagsValue;
        _boolResult = Singulink.Enums.EnumExtensions.AreFlagsDefined(value);
        _boolResult = Singulink.Enums.EnumExtensions.AreFlagsDefined(value);
        _boolResult = Singulink.Enums.EnumExtensions.AreFlagsDefined(value);
        _boolResult = Singulink.Enums.EnumExtensions.AreFlagsDefined(value);
        _boolResult = Singulink.Enums.EnumExtensions.AreFlagsDefined(value);
        _boolResult = Singulink.Enums.EnumExtensions.AreFlagsDefined(value);
        _boolResult = Singulink.Enums.EnumExtensions.AreFlagsDefined(value);
        _boolResult = Singulink.Enums.EnumExtensions.AreFlagsDefined(value);
        _boolResult = Singulink.Enums.EnumExtensions.AreFlagsDefined(value);
        _boolResult = Singulink.Enums.EnumExtensions.AreFlagsDefined(value);
    }

    [BenchmarkCategory("AreFlagsDefined"), Benchmark(OperationsPerInvoke = 10)]
    public void AreFlagsDefined_EnumsNet()
    {
        var value = _singleAllFlagsValue;
        _boolResult = EnumsNET.FlagEnums.IsValidFlagCombination(value);
        _boolResult = EnumsNET.FlagEnums.IsValidFlagCombination(value);
        _boolResult = EnumsNET.FlagEnums.IsValidFlagCombination(value);
        _boolResult = EnumsNET.FlagEnums.IsValidFlagCombination(value);
        _boolResult = EnumsNET.FlagEnums.IsValidFlagCombination(value);
        _boolResult = EnumsNET.FlagEnums.IsValidFlagCombination(value);
        _boolResult = EnumsNET.FlagEnums.IsValidFlagCombination(value);
        _boolResult = EnumsNET.FlagEnums.IsValidFlagCombination(value);
        _boolResult = EnumsNET.FlagEnums.IsValidFlagCombination(value);
        _boolResult = EnumsNET.FlagEnums.IsValidFlagCombination(value);
    }

    [BenchmarkCategory("SplitFlags"), Benchmark(Baseline = true)]
    public AttributeTargets SplitFlags_Singulink()
    {
        return Singulink.Enums.EnumExtensions.SplitFlags(_manySingleFlags)[0];
    }

    [BenchmarkCategory("SplitFlags"), Benchmark]
    public AttributeTargets SplitFlags_EnumsNet()
    {
        return EnumsNET.FlagEnums.GetFlags(_manySingleFlags)[0];
    }

    [BenchmarkCategory("ParseSingle"), Benchmark(Baseline = true)]
    public AttributeTargets ParseSingle_Singulink()
    {
        return Singulink.Enums.Enum<AttributeTargets>.Parse("All");
    }

    [BenchmarkCategory("ParseSingle"), Benchmark]
    public AttributeTargets ParseSingle_EnumsNet()
    {
        return EnumsNET.FlagEnums.ParseFlags<AttributeTargets>("All");
    }

    [BenchmarkCategory("ParseSingle"), Benchmark]
    public AttributeTargets ParseSingle_System()
    {
        return (AttributeTargets)Enum.Parse(typeof(AttributeTargets), "All");
    }

    [BenchmarkCategory("ParseSingleIgnoreCase"), Benchmark(Baseline = true)]
    public AttributeTargets ParseSingleIgnoreCase_Singulink()
    {
        return Singulink.Enums.Enum<AttributeTargets>.Parse("all", ignoreCase: true);
    }

    [BenchmarkCategory("ParseSingleIgnoreCase"), Benchmark]
    public AttributeTargets ParseSingleIgnoreCase_EnumsNet()
    {
        return EnumsNET.FlagEnums.ParseFlags<AttributeTargets>("all", ignoreCase: true);
    }

    [BenchmarkCategory("ParseSingleIgnoreCase"), Benchmark]
    public AttributeTargets ParseSingleIgnoreCase_System()
    {
        return (AttributeTargets)Enum.Parse(typeof(AttributeTargets), "all", true);
    }

    [BenchmarkCategory("ParseMultiple"), Benchmark(Baseline = true)]
    public AttributeTargets ParseMultiple_Singulink()
    {
        return Singulink.Enums.Enum<AttributeTargets>.Parse("Assembly, Class, Method");
    }

    [BenchmarkCategory("ParseMultiple"), Benchmark]
    public AttributeTargets ParseMultiple_EnumsNet()
    {
        return EnumsNET.FlagEnums.ParseFlags<AttributeTargets>("Assembly, Class, Method");
    }

    [BenchmarkCategory("ParseMultiple"), Benchmark]
    public AttributeTargets ParseMultiple_System()
    {
        return (AttributeTargets)Enum.Parse(typeof(AttributeTargets), "Assembly, Class, Method");
    }

    [BenchmarkCategory("ParseMultipleIgnoreCase"), Benchmark(Baseline = true)]
    public AttributeTargets ParseMultipleIgnoreCase_Singulink()
    {
        return Singulink.Enums.Enum<AttributeTargets>.Parse("assembly, class, method", ignoreCase: true);
    }

    [BenchmarkCategory("ParseMultipleIgnoreCase"), Benchmark]
    public AttributeTargets ParseMultipleIgnoreCase_EnumsNet()
    {
        return EnumsNET.FlagEnums.ParseFlags<AttributeTargets>("assembly, class, method", ignoreCase: true);
    }

    [BenchmarkCategory("ParseMultipleIgnoreCase"), Benchmark]
    public AttributeTargets ParseMultipleIgnoreCase_System()
    {
        return (AttributeTargets)Enum.Parse(typeof(AttributeTargets), "assembly, class, method", true);
    }

    [BenchmarkCategory("ToStringSingle"), Benchmark(Baseline = true)]
    public string AsStringSingle_Singulink()
    {
        return Singulink.Enums.EnumExtensions.AsString(_singleAllFlagsValue);
    }

    [BenchmarkCategory("ToStringSingle"), Benchmark]
    public string AsStringSingle_EnumsNet()
    {
        return EnumsNET.Enums.AsString(_singleAllFlagsValue);
    }

    [BenchmarkCategory("ToStringSingle"), Benchmark]
    public string AsStringSingle_System()
    {
        return _singleAllFlagsValue.ToString();
    }

    [BenchmarkCategory("ToStringMultiple"), Benchmark(Baseline = true)]
    public string AsStringMultiple_Singulink()
    {
        return Singulink.Enums.EnumExtensions.AsString(_manySingleFlags);
    }

    [BenchmarkCategory("ToStringMultiple"), Benchmark]
    public string AsStringMultiple_EnumsNet()
    {
        return EnumsNET.Enums.AsString(_manySingleFlags);
    }

    [BenchmarkCategory("ToStringMultiple"), Benchmark]
    public string AsStringMultiple_System()
    {
        return _manySingleFlags.ToString();
    }

    public static void Main()
    {
        Console.WriteLine("Press any key to begin...");
        Console.WriteLine();
        Console.ReadKey(true);

        BenchmarkRunner.Run<Program>(args: [
            "--unrollFactor", "64",
            "--anyCategories", "IsDefined",
            //"SplitFlags",
        ]);

        Console.ReadKey(true);
    }
}
