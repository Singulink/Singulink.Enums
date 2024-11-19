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
```

## Benchmarks (.NET 8.0)

The following is a comparison between Singulink Enums, Enums.Net (v5.0) and operators / system methods (where applicable). Some methods may have subtle behavioral differences.

```
| Method                            | Mean        | Error     | StdDev    |
|---------------------------------- |------------:|----------:|----------:|
| AreFlagsDefined_Singulink         |   0.0195 ns | 0.0001 ns | 0.0001 ns |
| AreFlagsDefined_EnumsNet          |   0.0225 ns | 0.0002 ns | 0.0002 ns |
|                                   |             |           |           |
| ClearFlags_Singulink              |   0.0118 ns | 0.0001 ns | 0.0001 ns |
| ClearFlags_EnumsNet               |   0.2314 ns | 0.0005 ns | 0.0004 ns |
| ClearFlags_Operator               |   0.0117 ns | 0.0002 ns | 0.0001 ns |
|                                   |             |           |           |
| HasAllFlags_Singulink             |   0.0196 ns | 0.0003 ns | 0.0003 ns |
| HasAllFlags_EnumsNet              |   0.3364 ns | 0.0005 ns | 0.0005 ns |
| HasAllFlags_System                |   0.0120 ns | 0.0002 ns | 0.0002 ns |
|                                   |             |           |           |
| HasAnyFlags_Singulink             |   0.0191 ns | 0.0002 ns | 0.0001 ns |
| HasAnyFlags_EnumsNet              |   0.2738 ns | 0.0004 ns | 0.0003 ns |
| HasAnyFlags_Operator              |   0.0123 ns | 0.0002 ns | 0.0002 ns |
|                                   |             |           |           |
| IsDefined_Singulink               |   0.3984 ns | 0.0008 ns | 0.0008 ns |
| IsDefined_EnumsNet                |   1.3387 ns | 0.0026 ns | 0.0024 ns |
| IsDefined_System                  |   2.0957 ns | 0.0064 ns | 0.0060 ns |
|                                   |             |           |           |
| ParseMultiple_Singulink           |  33.5060 ns | 0.1024 ns | 0.0958 ns |
| ParseMultiple_EnumsNet            |  51.4606 ns | 0.0518 ns | 0.0484 ns |
| ParseMultiple_System              |  51.4423 ns | 0.1366 ns | 0.1278 ns |
|                                   |             |           |           |
| ParseMultipleIgnoreCase_Singulink |  33.6575 ns | 0.0168 ns | 0.0157 ns |
| ParseMultipleIgnoreCase_EnumsNet  |  56.0421 ns | 0.1147 ns | 0.1073 ns |
| ParseMultipleIgnoreCase_System    |  50.4676 ns | 0.1079 ns | 0.1009 ns |
|                                   |             |           |           |
| ParseSingle_Singulink             |   7.5045 ns | 0.0265 ns | 0.0248 ns |
| ParseSingle_EnumsNet              |  15.5357 ns | 0.0273 ns | 0.0255 ns |
| ParseSingle_System                |  37.5409 ns | 0.1051 ns | 0.0932 ns |
|                                   |             |           |           |
| ParseSingleIgnoreCase_Singulink   |   7.4814 ns | 0.0347 ns | 0.0325 ns |
| ParseSingleIgnoreCase_EnumsNet    |  16.7586 ns | 0.0220 ns | 0.0206 ns |
| ParseSingleIgnoreCase_System      |  39.3521 ns | 0.0308 ns | 0.0273 ns |
|                                   |             |           |           |
| SetFlags_Singulink                |   0.0124 ns | 0.0003 ns | 0.0002 ns |
| SetFlags_EnumsNet                 |   0.2272 ns | 0.0001 ns | 0.0001 ns |
| SetFlags_Operator                 |   0.0121 ns | 0.0002 ns | 0.0002 ns |
|                                   |             |           |           |
| SplitFlags_Singulink              |  23.2878 ns | 0.0286 ns | 0.0254 ns |
| SplitFlags_EnumsNet               |  84.8990 ns | 0.1504 ns | 0.1407 ns |
|                                   |             |           |           |
| AsStringMultiple_Singulink        |  77.5238 ns | 0.1581 ns | 0.1402 ns |
| AsStringMultiple_EnumsNet         | 147.6367 ns | 0.4769 ns | 0.4461 ns |
| AsStringMultiple_System           |  47.0363 ns | 0.6862 ns | 0.6083 ns |
|                                   |             |           |           |
| AsStringSingle_Singulink          |   5.1679 ns | 0.0093 ns | 0.0082 ns |
| AsStringSingle_EnumsNet           |   4.0529 ns | 0.0087 ns | 0.0082 ns |
| AsStringSingle_System             |   5.6126 ns | 0.0113 ns | 0.0105 ns |
```
