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
bool hasAssemblyOrCtor = value.HasAnyFlag(AttributeTargets.Assembly, AttributeTargets.Constructor); // true
IReadOnlyList<AttributeTargets> splitValues = value.SplitFlags(); // 3 separate flags split out

// Create a case-insensitive converter with a non-default separator
var converter = new EnumConverter<AttributeTargets>(opt => {
    opt.Separator = " | ";
    opt.IgnoreCase = true;
});

// Convert enum value to/from string using the customized converter
string enumString = converter.AsString(value) // "Assembly | Class | Method"
value = converter.Parse(enumString);

// Create an enum converter that uses the [Display] attribute to get the names
var displayNameConverter = new EnumConverter<YourEnum>(opt => opt.WithDisplayNameGetter());
```

## Benchmarks (.NET 8.0)

The following is a comparison between Singulink Enums, Enums.Net (v5.0) and operators / system methods (where applicable). Some methods may have subtle behavioral differences.

```
| Method                            | Mean        | Error     | StdDev    |
|-----------------------------------|-------------|-----------|-----------|
| AreFlagsDefined_Singulink         |   0.1889 ns | 0.0025 ns | 0.0021 ns |
| AreFlagsDefined_EnumsNet          |   0.1739 ns | 0.0018 ns | 0.0017 ns |
|                                   |             |           |           |
| ClearFlags_Singulink              |   0.1365 ns | 0.0017 ns | 0.0016 ns |
| ClearFlags_EnumsNet               |   0.7752 ns | 0.0026 ns | 0.0024 ns |
| ClearFlags_Operator               |   0.1453 ns | 0.0024 ns | 0.0022 ns |
|                                   |             |           |           |
| HasAllFlags_Singulink             |   0.1586 ns | 0.0009 ns | 0.0008 ns |
| HasAllFlags_EnumsNet              |   0.9162 ns | 0.0035 ns | 0.0033 ns |
| HasAllFlags_System                |   0.1441 ns | 0.0016 ns | 0.0013 ns |
|                                   |             |           |           |
| HasAnyFlags_Singulink             |   0.1588 ns | 0.0015 ns | 0.0014 ns |
| HasAnyFlags_EnumsNet              |   0.7892 ns | 0.0037 ns | 0.0035 ns |
| HasAnyFlags_Operator              |   0.1358 ns | 0.0007 ns | 0.0006 ns |
|                                   |             |           |           |
| IsDefined_Singulink               |    1.242 ns | 0.0147 ns | 0.0123 ns |
| IsDefined_EnumsNet                |    4.298 ns | 0.0237 ns | 0.0198 ns |
| IsDefined_System                  |    6.919 ns | 0.0673 ns | 0.0630 ns |
|                                   |             |           |           |
| ParseMultiple_Singulink           |  97.5124 ns | 0.7262 ns | 0.6793 ns |
| ParseMultiple_EnumsNet            | 132.8245 ns | 0.3991 ns | 0.3733 ns |
| ParseMultiple_System              | 174.0145 ns | 0.7984 ns | 0.7078 ns |
|                                   |             |           |           |
| ParseMultipleIgnoreCase_Singulink |  97.0535 ns | 0.3031 ns | 0.2687 ns |
| ParseMultipleIgnoreCase_EnumsNet  | 153.8227 ns | 0.8173 ns | 0.7645 ns |
| ParseMultipleIgnoreCase_System    | 150.5741 ns | 0.4337 ns | 0.3845 ns |
|                                   |             |           |           |
| ParseSingle_Singulink             |  18.8381 ns | 0.2114 ns | 0.1874 ns |
| ParseSingle_EnumsNet              |  42.9609 ns | 0.2629 ns | 0.2053 ns |
| ParseSingle_System                | 132.8030 ns | 0.9795 ns | 0.9162 ns |
|                                   |             |           |           |
| ParseSingleIgnoreCase_Singulink   |  20.0159 ns | 0.1196 ns | 0.1060 ns |
| ParseSingleIgnoreCase_EnumsNet    |  48.8390 ns | 0.4490 ns | 0.3506 ns |
| ParseSingleIgnoreCase_System      | 127.2730 ns | 0.7425 ns | 0.6200 ns |
|                                   |             |           |           |
| SetFlags_Singulink                |   0.1361 ns | 0.0015 ns | 0.0013 ns |
| SetFlags_EnumsNet                 |   0.9429 ns | 0.0135 ns | 0.0120 ns |
| SetFlags_Operator                 |   0.1351 ns | 0.0016 ns | 0.0013 ns |
|                                   |             |           |           |
| SplitFlags_Singulink              |  72.9657 ns | 0.9375 ns | 0.8769 ns |
| SplitFlags_EnumsNet               | 240.1368 ns | 4.3469 ns | 3.8534 ns |
|                                   |             |           |           |
| AsStringMultiple_Singulink        | 225.2603 ns | 3.2194 ns | 2.8540 ns |
| AsStringMultiple_EnumsNet         | 466.7276 ns | 1.9668 ns | 1.6423 ns |
| AsStringMultiple_System           | 137.1171 ns | 1.8917 ns | 1.5796 ns |
|                                   |             |           |           |
| AsStringSingle_Singulink          |  14.6719 ns | 0.1029 ns | 0.0859 ns |
| AsStringSingle_EnumsNet           |  10.1860 ns | 0.0632 ns | 0.0528 ns |
| AsStringSingle_System             |  18.6756 ns | 0.0746 ns | 0.0623 ns |
|-----------------------------------|-------------|-----------|-----------|
```
