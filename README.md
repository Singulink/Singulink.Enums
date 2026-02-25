# Singulink Enums

[![Chat on Discord](https://img.shields.io/discord/906246067773923490)](https://discord.gg/EkQhJFsBu6)
[![View nuget packages](https://img.shields.io/nuget/v/Singulink.Enums.svg)](https://www.nuget.org/packages/Singulink.Enums/)
[![Build and Test](https://github.com/Singulink/Singulink.Enums/workflows/build%20and%20test/badge.svg)](https://github.com/Singulink/Singulink.Enums/actions?query=workflow%3A%22build+and+test%22)

**Singulink Enums** is a tiny (~30KB), highly optimized library with full AOT support that provides generic operations and extension methods for enumeration types. It contains a comprehensive set of the most common enumeration "must haves" that are missing from the .NET runtime. The effect on runtime memory footprint has been pushed to the bare minimum while still supporting common scenarios in the most efficient way possible.

### About Singulink

We are a small team of engineers and designers dedicated to building beautiful, functional and well-engineered software solutions. We offer very competitive rates as well as fixed-price contracts and welcome inquiries to discuss any custom development / project support needs you may have.

This package is part of our **Singulink Libraries** collection. Visit https://github.com/Singulink to see our full list of publicly available libraries and other open-source projects.

## Installation

The package is available on NuGet - simply install the `Singulink.Enums` package.

**Supported Runtimes**: Everywhere .NET Standard 2.0 is supported, including:
- .NET
- .NET Framework
- Mono / Xamarin

## API

You can view the fully documented API on the [project documentation site](https://www.singulink.com/Docs/Singulink.Enums/api/Singulink.Enums.html).

The main classes of interest are:
1. `Enum<T>` - Static generic helper properties and methods when the enum type needs to specified, i.e. for parsing.
2. `EnumExtensions` - Extension methods to validate enums and perform operations on values.
3. `EnumConverter<T>` - Enum converter that can convert enums to and from strings. Separators, enum names and case-sensitivity are fully customizable.

## Usage

The API is fairly self-explanatory and well documented but here are some examples to show what the library offers:

```c#
using Singulink.Enums;

// Get enum names
var enumNames = Enum<ConsoleKey>.Names;

// Get enum values
var enumValues = Enum<ConsoleKey>.Values;

// Get enum field name for a particular value
string backspaceName = ConsoleKey.Backspace.GetName();

// Parse using the default converter
ConsoleKey backspace = Enum<ConsoleKey>.Parse("Backspace");

// Bitwise flag operations
var value = AttributeTargets.Assembly;
value = value.SetFlags(AttributeTargets.Class, AttributeTargets.Method); // set additional flags
bool hasClassAndMethod = value.HasAllFlags(AttributeTargets.Class, AttributeTargets.Method); // true
bool hasClassOrStruct = value.HasAnyFlag(AttributeTargets.Class, AttributeTargets.Struct); // true
IReadOnlyList<AttributeTargets> splitValues = value.SplitFlags(); // 3 separate flags split out

// Create a case-insensitive converter with a non-default separator
var converter = new EnumConverter<AttributeTargets>(opt => {
    opt.Separator = " | ";
    opt.IgnoreCase = true;
});

// Convert enum value to/from string using the customized converter
string enumString = converter.AsString(value) // "Assembly | Class | Method"
value = converter.Parse(enumString.ToLowerInvariant()); // Assembly, Class and Method flags set

// Create an enum converter that uses the [Display] attribute to get the names
var displayNameConverter = new EnumConverter<YourEnum>(opt => opt.WithDisplayNameGetter());

public void Method(AttributeTargets targets)
{
    // Throw ArgumentOutOfRangeException if targets is not a valid flag combo
    targets.ThrowIfFlagsAreNotDefined(nameof(targets));
}

public void Method(ConsoleKey key)
{
    // Throw ArgumentOutOfRangeException if key is not defined
    key.ThrowIfNotDefined(nameof(key));
}
```

## Benchmarks (.NET 10.0)

The following is a comparison between Singulink Enums, Enums.Net (v5.0) and operators / system methods (where applicable):

```
| Method                            | Mean        | Error     | StdDev    |
|---------------------------------- |------------:|----------:|----------:|
| AreFlagsDefined_Singulink         |   0.0155 ns | 0.0002 ns | 0.0002 ns |
| AreFlagsDefined_EnumsNet          |   0.0228 ns | 0.0003 ns | 0.0003 ns |
|                                   |             |           |           |
| ClearFlags_Singulink              |   0.0003 ns | 0.0001 ns | 0.0001 ns |
| ClearFlags_EnumsNet               |   0.2725 ns | 0.0006 ns | 0.0006 ns |
| ClearFlags_Operator               |   0.0004 ns | 0.0001 ns | 0.0001 ns |
|                                   |             |           |           |
| HasAllFlags_Singulink             |   0.0003 ns | 0.0001 ns | 0.0001 ns |
| HasAllFlags_EnumsNet              |   0.3540 ns | 0.0068 ns | 0.0064 ns |
| HasAllFlags_System                |   0.0140 ns | 0.0004 ns | 0.0003 ns |
|                                   |             |           |           |
| HasAnyFlags_Singulink             |   0.0004 ns | 0.0005 ns | 0.0005 ns |
| HasAnyFlags_EnumsNet              |   0.2976 ns | 0.0015 ns | 0.0014 ns |
| HasAnyFlags_Operator              |   0.0004 ns | 0.0006 ns | 0.0006 ns |
|                                   |             |           |           |
| IsDefined_Singulink               |   0.3997 ns | 0.0005 ns | 0.0004 ns |
| IsDefined_EnumsNet                |   1.3685 ns | 0.0021 ns | 0.0018 ns |
| IsDefined_System                  |   1.7787 ns | 0.0036 ns | 0.0030 ns |
|                                   |             |           |           |
| ParseMultiple_Singulink           |  22.1824 ns | 0.0206 ns | 0.0172 ns |
| ParseMultiple_EnumsNet            |  42.3496 ns | 0.0936 ns | 0.0829 ns |
| ParseMultiple_System              |  26.6418 ns | 0.0716 ns | 0.0670 ns |
|                                   |             |           |           |
| ParseMultipleIgnoreCase_Singulink |  30.6828 ns | 0.1017 ns | 0.0902 ns |
| ParseMultipleIgnoreCase_EnumsNet  |  55.5716 ns | 0.1233 ns | 0.0963 ns |
| ParseMultipleIgnoreCase_System    |  30.4658 ns | 0.1801 ns | 0.1597 ns |
|                                   |             |           |           |
| ParseSingle_Singulink             |   5.4175 ns | 0.0180 ns | 0.0160 ns |
| ParseSingle_EnumsNet              |  14.4765 ns | 0.0075 ns | 0.0063 ns |
| ParseSingle_System                |  14.9761 ns | 0.1464 ns | 0.1370 ns |
|                                   |             |           |           |
| ParseSingleIgnoreCase_Singulink   |   6.5166 ns | 0.0203 ns | 0.0169 ns |
| ParseSingleIgnoreCase_EnumsNet    |  14.3623 ns | 0.0132 ns | 0.0103 ns |
| ParseSingleIgnoreCase_System      |  15.8176 ns | 0.0566 ns | 0.0529 ns |
|                                   |             |           |           |
| SetFlags_Singulink                |   0.0002 ns | 0.0001 ns | 0.0001 ns |
| SetFlags_EnumsNet                 |   0.2783 ns | 0.0006 ns | 0.0005 ns |
| SetFlags_Operator                 |   0.0003 ns | 0.0002 ns | 0.0002 ns |
|                                   |             |           |           |
| SplitFlags_Singulink*             |  20.5384 ns | 0.0313 ns | 0.0244 ns |
| SplitFlags_EnumsNet               |  76.3367 ns | 0.8440 ns | 0.7047 ns |
|                                   |             |           |           |
| AsStringMultiple_Singulink*       |  55.9299 ns | 0.2222 ns | 0.1970 ns |
| AsStringMultiple_EnumsNet         | 162.1769 ns | 0.7477 ns | 0.6244 ns |
| AsStringMultiple_System           |  45.5259 ns | 0.3624 ns | 0.3212 ns |
|                                   |             |           |           |
| AsStringSingle_Singulink          |   3.1599 ns | 0.0192 ns | 0.0170 ns |
| AsStringSingle_EnumsNet           |   2.3536 ns | 0.0131 ns | 0.0116 ns |
| AsStringSingle_System             |   5.7745 ns | 0.0303 ns | 0.0253 ns |
```

\* Split flags and flags converted to strings are returned in ascending order (based on the highest bits set) with any remainder added to the end. This behavior differs from Enums.Net and system methods as this is usually what developers want. You also have the option to return all matching flags (v.s. just the minimal set of flags that can be combined to form the original value) or splitting only using single-bit values. See [`SplitFlagsOptions`](https://www.singulink.com/Docs/Singulink.Enums/api/Singulink.Enums.SplitFlagsOptions.html).
