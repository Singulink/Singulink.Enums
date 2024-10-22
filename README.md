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

## Benchmarks

The following is a comparison between **Singulink Enums**, **Enums.Net** and operators / system methods (where applicable).

```
| Method                            | Mean        | Error     | StdDev    | Median      | Ratio | RatioSD |
|---------------------------------- |------------:|----------:|----------:|------------:|------:|--------:|
| ClearFlags_Singulink              |   0.3314 ns | 0.0201 ns | 0.0167 ns |   0.3247 ns |  1.00 |    0.07 |
| ClearFlags_EnumsNet               |   3.4567 ns | 0.0182 ns | 0.0171 ns |   3.4548 ns | 10.45 |    0.49 |
| ClearFlags_Operator               |   0.3009 ns | 0.0072 ns | 0.0064 ns |   0.3002 ns |  0.91 |    0.05 |
|                                   |             |           |           |             |       |         |
| HasAllFlags_Singulink             |   0.4781 ns | 0.0050 ns | 0.0044 ns |   0.4785 ns |  1.00 |    0.01 |
| HasAllFlags_EnumsNet              |   4.6582 ns | 0.0152 ns | 0.0127 ns |   4.6558 ns |  9.74 |    0.09 |
| HasAllFlags_System                |   0.3038 ns | 0.0073 ns | 0.0061 ns |   0.3048 ns |  0.64 |    0.01 |
|                                   |             |           |           |             |       |         |
| HasAnyFlags_Singulink             |   0.3327 ns | 0.0086 ns | 0.0077 ns |   0.3297 ns |  1.00 |    0.03 |
| HasAnyFlags_EnumsNet              |   3.6724 ns | 0.0147 ns | 0.0130 ns |   3.6734 ns | 11.04 |    0.24 |
| HasAnyFlags_Operator              |   0.2940 ns | 0.0054 ns | 0.0048 ns |   0.2948 ns |  0.88 |    0.02 |
|                                   |             |           |           |             |       |         |
| IsValid_Singulink                 |   0.7685 ns | 0.0161 ns | 0.0126 ns |   0.7682 ns |  1.00 |    0.02 |
| IsValid_EnumsNet                  |   0.6288 ns | 0.0473 ns | 0.0631 ns |   0.6351 ns |  0.82 |    0.08 |
|                                   |             |           |           |             |       |         |
| ParseMultiple_Singulink           | 107.9728 ns | 2.1853 ns | 5.3607 ns | 105.3228 ns |  1.00 |    0.07 |
| ParseMultiple_EnumsNet            | 135.5901 ns | 2.7571 ns | 6.1667 ns | 133.1990 ns |  1.26 |    0.08 |
| ParseMultiple_System              | 161.6867 ns | 1.3217 ns | 1.1717 ns | 161.3133 ns |  1.50 |    0.07 |
|                                   |             |           |           |             |       |         |
| ParseMultipleIgnoreCase_Singulink |  97.2899 ns | 0.5085 ns | 0.4507 ns |  97.2672 ns |  1.00 |    0.01 |
| ParseMultipleIgnoreCase_EnumsNet  | 161.8514 ns | 0.5155 ns | 0.4305 ns | 161.7216 ns |  1.66 |    0.01 |
| ParseMultipleIgnoreCase_System    | 151.1973 ns | 0.8824 ns | 0.7822 ns | 151.0300 ns |  1.55 |    0.01 |
|                                   |             |           |           |             |       |         |
| ParseSingle_Singulink             |  18.1077 ns | 0.0523 ns | 0.0464 ns |  18.0994 ns |  1.00 |    0.00 |
| ParseSingle_EnumsNet              |  42.8778 ns | 0.2553 ns | 0.2388 ns |  42.8819 ns |  2.37 |    0.01 |
| ParseSingle_System                | 134.3959 ns | 0.9786 ns | 0.9153 ns | 134.0643 ns |  7.42 |    0.05 |
|                                   |             |           |           |             |       |         |
| ParseSingleIgnoreCase_Singulink   |  19.8005 ns | 0.0750 ns | 0.0665 ns |  19.7854 ns |  1.00 |    0.00 |
| ParseSingleIgnoreCase_EnumsNet    |  49.3837 ns | 0.9668 ns | 0.9495 ns |  49.4232 ns |  2.49 |    0.05 |
| ParseSingleIgnoreCase_System      | 134.0639 ns | 2.7001 ns | 3.7851 ns | 133.3476 ns |  6.77 |    0.19 |
|                                   |             |           |           |             |       |         |
| SetFlags_Singulink                |   0.3975 ns | 0.0340 ns | 0.0731 ns |   0.3801 ns |  1.03 |    0.24 |
| SetFlags_EnumsNet                 |   3.3298 ns | 0.0180 ns | 0.0168 ns |   3.3315 ns |  8.61 |    1.29 |
| SetFlags_Operator                 |   0.4664 ns | 0.0226 ns | 0.0189 ns |   0.4636 ns |  1.21 |    0.19 |
|                                   |             |           |           |             |       |         |
| SplitFlags_Singulink              |  71.5438 ns | 0.4925 ns | 0.4365 ns |  71.5195 ns |  1.00 |    0.01 |
| SplitFlags_EnumsNet               | 244.8229 ns | 2.1826 ns | 1.9348 ns | 245.0448 ns |  3.42 |    0.03 |
|                                   |             |           |           |             |       |         |
| ToStringMultiple_Singulink        | 224.2061 ns | 1.3623 ns | 1.2743 ns | 223.9949 ns |  1.00 |    0.01 |
| ToStringMultiple_EnumsNet         | 474.6005 ns | 3.2677 ns | 2.8967 ns | 474.0946 ns |  2.12 |    0.02 |
| ToStringMultiple_System           | 138.3636 ns | 0.7676 ns | 0.6805 ns | 138.2386 ns |  0.62 |    0.00 |
|                                   |             |           |           |             |       |         |
| ToStringSingle_Singulink          |  11.2825 ns | 0.0596 ns | 0.0528 ns |  11.2871 ns |  1.00 |    0.01 |
| ToStringSingle_EnumsNet           |  11.0774 ns | 0.1271 ns | 0.1062 ns |  11.0484 ns |  0.98 |    0.01 |
| ToStringSingle_System             |  19.4789 ns | 0.1515 ns | 0.1417 ns |  19.4448 ns |  1.73 |    0.01 |
```
