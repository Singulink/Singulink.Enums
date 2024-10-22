using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

#pragma warning disable SA1005
#pragma warning disable SA1515

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class Program
{
    private readonly AttributeTargets _singleDefinedValue = AttributeTargets.All;
    private readonly AttributeTargets _multipleDefinedValues = AttributeTargets.All & ~AttributeTargets.Assembly;

    private bool _boolResult;
    private AttributeTargets _enumResult;

    [BenchmarkCategory("HasAllFlags"), Benchmark(Baseline = true)]
    public void HasAllFlags_Singulink()
    {
        _boolResult = Singulink.Enums.EnumExtensions.HasAllFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAllFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAllFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAllFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAllFlags(_singleDefinedValue, AttributeTargets.Assembly);
    }

    [BenchmarkCategory("HasAllFlags"), Benchmark]
    public void HasAllFlags_EnumsNet()
    {
        _boolResult = EnumsNET.FlagEnums.HasAllFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAllFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAllFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAllFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAllFlags(_singleDefinedValue, AttributeTargets.Assembly);
    }

    [BenchmarkCategory("HasAllFlags"), Benchmark]
    public void HasAllFlags_System()
    {
        _boolResult = _singleDefinedValue.HasFlag(AttributeTargets.Assembly);
        _boolResult = _singleDefinedValue.HasFlag(AttributeTargets.Assembly);
        _boolResult = _singleDefinedValue.HasFlag(AttributeTargets.Assembly);
        _boolResult = _singleDefinedValue.HasFlag(AttributeTargets.Assembly);
        _boolResult = _singleDefinedValue.HasFlag(AttributeTargets.Assembly);
    }

    [BenchmarkCategory("HasAnyFlags"), Benchmark(Baseline = true)]
    public void HasAnyFlags_Singulink()
    {
        _boolResult = Singulink.Enums.EnumExtensions.HasAnyFlag(_singleDefinedValue, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAnyFlag(_singleDefinedValue, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAnyFlag(_singleDefinedValue, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAnyFlag(_singleDefinedValue, AttributeTargets.Assembly);
        _boolResult = Singulink.Enums.EnumExtensions.HasAnyFlag(_singleDefinedValue, AttributeTargets.Assembly);
    }

    [BenchmarkCategory("HasAnyFlags"), Benchmark]
    public void HasAnyFlags_EnumsNet()
    {
        _boolResult = EnumsNET.FlagEnums.HasAnyFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAnyFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAnyFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAnyFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _boolResult = EnumsNET.FlagEnums.HasAnyFlags(_singleDefinedValue, AttributeTargets.Assembly);
    }

    [BenchmarkCategory("HasAnyFlags"), Benchmark]
    public void HasAnyFlags_Operator()
    {
        _boolResult = (_singleDefinedValue & AttributeTargets.Assembly) != 0;
        _boolResult = (_singleDefinedValue & AttributeTargets.Assembly) != 0;
        _boolResult = (_singleDefinedValue & AttributeTargets.Assembly) != 0;
        _boolResult = (_singleDefinedValue & AttributeTargets.Assembly) != 0;
        _boolResult = (_singleDefinedValue & AttributeTargets.Assembly) != 0;
    }

    [BenchmarkCategory("SetFlags"), Benchmark(Baseline = true)]
    public void SetFlags_Singulink()
    {
        _enumResult = Singulink.Enums.EnumExtensions.SetFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.SetFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.SetFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.SetFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.SetFlags(_singleDefinedValue, AttributeTargets.Assembly);
    }

    [BenchmarkCategory("SetFlags"), Benchmark]
    public void SetFlags_EnumsNet()
    {
        _enumResult = EnumsNET.FlagEnums.CombineFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.CombineFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.CombineFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.CombineFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.CombineFlags(_singleDefinedValue, AttributeTargets.Assembly);
    }

    [BenchmarkCategory("SetFlags"), Benchmark]
    public void SetFlags_Operator()
    {
        _enumResult = _singleDefinedValue | AttributeTargets.Assembly;
        _enumResult = _singleDefinedValue | AttributeTargets.Assembly;
        _enumResult = _singleDefinedValue | AttributeTargets.Assembly;
        _enumResult = _singleDefinedValue | AttributeTargets.Assembly;
        _enumResult = _singleDefinedValue | AttributeTargets.Assembly;
    }

    [BenchmarkCategory("ClearFlags"), Benchmark(Baseline = true)]
    public void ClearFlags_Singulink()
    {
        _enumResult = Singulink.Enums.EnumExtensions.ClearFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.ClearFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.ClearFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.ClearFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _enumResult = Singulink.Enums.EnumExtensions.ClearFlags(_singleDefinedValue, AttributeTargets.Assembly);
    }

    [BenchmarkCategory("ClearFlags"), Benchmark]
    public void ClearFlags_EnumsNet()
    {
        _enumResult = EnumsNET.FlagEnums.RemoveFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.RemoveFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.RemoveFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.RemoveFlags(_singleDefinedValue, AttributeTargets.Assembly);
        _enumResult = EnumsNET.FlagEnums.RemoveFlags(_singleDefinedValue, AttributeTargets.Assembly);
    }

    [BenchmarkCategory("ClearFlags"), Benchmark]
    public void ClearFlags_Operator()
    {
        _enumResult = _singleDefinedValue & ~AttributeTargets.Assembly;
        _enumResult = _singleDefinedValue & ~AttributeTargets.Assembly;
        _enumResult = _singleDefinedValue & ~AttributeTargets.Assembly;
        _enumResult = _singleDefinedValue & ~AttributeTargets.Assembly;
        _enumResult = _singleDefinedValue & ~AttributeTargets.Assembly;
    }

    [BenchmarkCategory("SplitFlags"), Benchmark(Baseline = true)]
    public AttributeTargets SplitFlags_Singulink()
    {
        return Singulink.Enums.EnumExtensions.SplitFlags(_multipleDefinedValues)[0];
    }

    [BenchmarkCategory("SplitFlags"), Benchmark]
    public AttributeTargets SplitFlags_EnumsNet()
    {
        return EnumsNET.FlagEnums.GetFlags(_multipleDefinedValues)[0];
    }

    [BenchmarkCategory("IsValid"), Benchmark(Baseline = true)]
    public bool IsValid_Singulink()
    {
        return Singulink.Enums.EnumExtensions.IsValid(_singleDefinedValue);
    }

    [BenchmarkCategory("IsValid"), Benchmark]
    public bool IsValid_EnumsNet()
    {
        return EnumsNET.Enums.IsValid(_singleDefinedValue);
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
    public string ToStringSingle_Singulink()
    {
        return Singulink.Enums.EnumExtensions.AsString(_singleDefinedValue);
    }

    [BenchmarkCategory("ToStringSingle"), Benchmark]
    public string ToStringSingle_EnumsNet()
    {
        return EnumsNET.Enums.AsString(_singleDefinedValue);
    }

    [BenchmarkCategory("ToStringSingle"), Benchmark]
    public string ToStringSingle_System()
    {
        return _singleDefinedValue.ToString();
    }

    [BenchmarkCategory("ToStringMultiple"), Benchmark(Baseline = true)]
    public string ToStringMultiple_Singulink()
    {
        return Singulink.Enums.EnumExtensions.AsString(_multipleDefinedValues);
    }

    [BenchmarkCategory("ToStringMultiple"), Benchmark]
    public string ToStringMultiple_EnumsNet()
    {
        return EnumsNET.Enums.AsString(_multipleDefinedValues);
    }

    [BenchmarkCategory("ToStringMultiple"), Benchmark]
    public string ToStringMultiple_System()
    {
        return _multipleDefinedValues.ToString();
    }

    public static void Main()
    {
        Console.WriteLine("Press any key to begin...");
        Console.WriteLine();
        Console.ReadKey(true);

        BenchmarkRunner.Run<Program>(args: new[] { "--unrollFactor", "64" });
        Console.ReadKey(true);
    }
}
