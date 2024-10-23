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
| Method                            | Mean        | Error     | StdDev    |
|---------------------------------- |------------:|----------:|----------:|
| ClearFlags_Singulink              |   0.3314 ns | 0.0201 ns | 0.0167 ns |
| ClearFlags_EnumsNet               |   3.4567 ns | 0.0182 ns | 0.0171 ns |
| ClearFlags_Operator               |   0.3009 ns | 0.0072 ns | 0.0064 ns |
|                                   |             |           |           |
| HasAllFlags_Singulink             |   0.3314 ns | 0.0420 ns | 0.0393 ns |
| HasAllFlags_EnumsNet              |   4.7759 ns | 0.0698 ns | 0.0619 ns |
| HasAllFlags_System                |   0.3299 ns | 0.0134 ns | 0.0112 ns |
|                                   |             |           |           |
| HasAnyFlags_Singulink             |   0.3327 ns | 0.0086 ns | 0.0077 ns |
| HasAnyFlags_EnumsNet              |   3.6724 ns | 0.0147 ns | 0.0130 ns |
| HasAnyFlags_Operator              |   0.2940 ns | 0.0054 ns | 0.0048 ns |
|                                   |             |           |           |
| IsValid_Singulink                 |   0.7685 ns | 0.0161 ns | 0.0126 ns |
| IsValid_EnumsNet                  |   0.6288 ns | 0.0473 ns | 0.0631 ns |
|                                   |             |           |           |
| ParseMultiple_Singulink           | 107.9728 ns | 2.1853 ns | 5.3607 ns |
| ParseMultiple_EnumsNet            | 135.5901 ns | 2.7571 ns | 6.1667 ns |
| ParseMultiple_System              | 161.6867 ns | 1.3217 ns | 1.1717 ns |
|                                   |             |           |           |
| ParseMultipleIgnoreCase_Singulink |  97.2899 ns | 0.5085 ns | 0.4507 ns |
| ParseMultipleIgnoreCase_EnumsNet  | 161.8514 ns | 0.5155 ns | 0.4305 ns |
| ParseMultipleIgnoreCase_System    | 151.1973 ns | 0.8824 ns | 0.7822 ns |
|                                   |             |           |           |
| ParseSingle_Singulink             |  18.1077 ns | 0.0523 ns | 0.0464 ns |
| ParseSingle_EnumsNet              |  42.8778 ns | 0.2553 ns | 0.2388 ns |
| ParseSingle_System                | 134.3959 ns | 0.9786 ns | 0.9153 ns |
|                                   |             |           |           |
| ParseSingleIgnoreCase_Singulink   |  19.8005 ns | 0.0750 ns | 0.0665 ns |
| ParseSingleIgnoreCase_EnumsNet    |  49.3837 ns | 0.9668 ns | 0.9495 ns |
| ParseSingleIgnoreCase_System      | 134.0639 ns | 2.7001 ns | 3.7851 ns |
|                                   |             |           |           |
| SetFlags_Singulink                |   0.3975 ns | 0.0340 ns | 0.0731 ns |
| SetFlags_EnumsNet                 |   3.3298 ns | 0.0180 ns | 0.0168 ns |
| SetFlags_Operator                 |   0.4664 ns | 0.0226 ns | 0.0189 ns |
|                                   |             |           |           |
| SplitFlags_Singulink              |  71.5438 ns | 0.4925 ns | 0.4365 ns |
| SplitFlags_EnumsNet               | 244.8229 ns | 2.1826 ns | 1.9348 ns |
|                                   |             |           |           |
| ToStringMultiple_Singulink        | 224.2061 ns | 1.3623 ns | 1.2743 ns |
| ToStringMultiple_EnumsNet         | 474.6005 ns | 3.2677 ns | 2.8967 ns |
| ToStringMultiple_System           | 138.3636 ns | 0.7676 ns | 0.6805 ns |
|                                   |             |           |           |
| ToStringSingle_Singulink          |  11.2825 ns | 0.0596 ns | 0.0528 ns |
| ToStringSingle_EnumsNet           |  11.0774 ns | 0.1271 ns | 0.1062 ns |
| ToStringSingle_System             |  19.4789 ns | 0.1515 ns | 0.1417 ns |
```
