# Singulink.Enums

[![Join the chat](https://badges.gitter.im/Singulink/community.svg)](https://gitter.im/Singulink/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![View nuget packages](https://img.shields.io/nuget/v/Singulink.Enums.svg)](https://www.nuget.org/packages/Singulink.Enums/)
[![Build and Test](https://github.com/Singulink/Singulink.Enums/workflows/build%20and%20test/badge.svg)](https://github.com/Singulink/Singulink.Enums/actions?query=workflow%3A%22build+and+test%22)

**Singulink.Enums** is a tiny (~20KB) library that provides generic operations and extension methods for enumeration types. It contains a minimal set of the most common enumeration "must haves" that are missing from the .NET runtime. The effect on runtime memory footprint has been pushed to the bare minimum while still supporting common scenarios in a highly optimized fashion.

### About Singulink

*Shameless plug*: We are a small team of engineers and designers dedicated to building beautiful, functional and well-engineered software solutions. We offer very competitive rates as well as fixed-price contracts and welcome inquiries to discuss any custom development / project support needs you may have.

This package is part of our **Singulink Libraries** collection. Visit https://github.com/Singulink to see our full list of publicly available libraries and other open-source projects.

## Installation

The package is available on NuGet - simply install the `Singulink.Enums` package.

**Supported Runtimes**: Anywhere .NET Standard 2.0+ is supported, including:
- .NET Core 2.0+
- .NET Framework 4.6.1+
- Mono 5.4+
- Xamarin.iOS 10.14+
- Xamarin.Android 8.0+

## API

You can view the API on [FuGet](https://www.fuget.org/packages/Singulink.Enums). The main classes of interest are:
1. `Enum<T>` - Static generic helper properties and methods to get cached enum info.
2. `EnumExtensions` - Extension methods to validate enums and perform bitwise operations.
3. `EnumParser<T>` - Fully customizable parser that can convert enums to and from strings with O(1) performance characteristics.

## Usage

The API is fairly self-explanatory and well documented but here are some examples to show what the library offers:

```c#
// Get enum names
var enumNames = Enum<ConsoleKey>.Names;

// Get enum values
var enumValues = Enum<ConsoleKey>.Values;

// Get enum field name for a particular value
string backspaceName = ConsoleKey.Backspace.GetName();

// Bitwise flag operations
var value = AttributeTargets.Assembly;
value = value.SetFlags(AttributeTargets.Class, AttributeTargets.Method);
bool hasClassAndMethod = value.HasAllFlags(AttributeTargets.Class, AttributeTargets.Method); // true
bool hasAssembly = value.HasAnyFlag(AttributeTargets.Assembly, AttributeTargets.Constructor); // true
IEnumerable<AttributeTargets> splitValues = value.SplitFlags(); // 3 separate flags split out

// Create a parser that uses the [Display] attribute to get the names
var displayNameParser = new EnumParser<MyEnum>(
    m => m.Field.GetCustomAttribute<DisplayAttribute>().GetName());

// Create a case-insensitive parser with a non-default separator
var parser = new EnumParser<AttributeTargets>(separator: " | ", caseSensitive: false);

// Convert enum value to/from string representation
string enumString = parser.ToString(value) // "Assembly | Class | Method"
value = parser.Parse(enumString);
```