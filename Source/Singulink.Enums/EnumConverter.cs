using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Singulink.Enums;

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

/// <summary>
/// Provides customizable enumeration string conversion functionality. All operations are thread-safe.
/// </summary>
public sealed class EnumConverter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>
    where T : unmanaged, Enum
{
    private static EnumConverter<T>? _default;
    private static EnumConverter<T>? _defaultIgnoreCase;
    private static EnumConverter<T>? _description;
    private static EnumConverter<T>? _descriptionIgnoreCase;
    private static EnumConverter<T>? _enumMemberValue;
    private static EnumConverter<T>? _enumMemberValueIgnoreCase;

    /// <summary>
    /// Gets the default case-sensitive enumeration converter that uses the <see cref="EnumFormat.Name"/> format.
    /// </summary>
    public static EnumConverter<T> Default => _default ??= new EnumConverter<T>(new EnumConvertOptions());

    /// <summary>
    /// Gets the default case-insensitive enumeration converter that uses the <see cref="EnumFormat.Name"/> format.
    /// </summary>
    public static EnumConverter<T> DefaultIgnoreCase => _defaultIgnoreCase ??= new EnumConverter<T>(new EnumConvertOptions() { IgnoreCase = true });

    /// <summary>
    /// Gets the shared default enumeration converter that uses the specified format and case-sensitivity. Converters that use the <see
    /// cref="EnumFormat.DisplayName"/> format for enumerations with localized display names are specific to the current <see
    /// cref="CultureInfo.CurrentUICulture"/>.
    /// </summary>
    /// <param name="format">The source of the enumeration names.</param>
    /// <param name="ignoreCase"><see langword="true"/> for case-insensitive or <see langword="false"/> for case-sensitive parsing.</param>
    public static EnumConverter<T> GetDefault(EnumFormat format, bool ignoreCase = false) => GetDefaultCore(format, null, ignoreCase);

    /// <summary>
    /// Gets the shared default enumeration converter that uses the specified format, culture and case-sensitivity.
    /// </summary>
    /// <param name="format">The source of the enumeration names.</param>
    /// <param name="culture">The culture used to localize names. Only used by the <see cref="EnumFormat.DisplayName"/> format for enumerations with
    /// localized display names, and ignored otherwise.</param>
    /// <param name="ignoreCase"><see langword="true"/> for case-insensitive or <see langword="false"/> for case-sensitive parsing.</param>
    public static EnumConverter<T> GetDefault(EnumFormat format, CultureInfo culture, bool ignoreCase = false)
    {
        return GetDefaultCore(format, culture, ignoreCase);
    }

    private static EnumConverter<T> GetDefaultCore(EnumFormat format, CultureInfo? culture, bool ignoreCase)
    {
        switch (format)
        {
            case EnumFormat.Name:
                return ignoreCase ? DefaultIgnoreCase : Default;
            case EnumFormat.DisplayName:
                return DisplayNameConverters.Get(culture, ignoreCase);
            case EnumFormat.Description:
                return ignoreCase ?
                    (_descriptionIgnoreCase ??= Create(format, ignoreCase)) :
                    (_description ??= Create(format, ignoreCase));
            case EnumFormat.EnumMemberValue:
                return ignoreCase ?
                    (_enumMemberValueIgnoreCase ??= Create(format, ignoreCase)) :
                    (_enumMemberValue ??= Create(format, ignoreCase));
            default:
                [DoesNotReturn]
                static void Throw() => throw new ArgumentOutOfRangeException(nameof(format), "Unsupported enumeration format.");
                Throw();
                return null;
        }

        static EnumConverter<T> Create(EnumFormat format, bool ignoreCase) => new(new EnumConvertOptions() { IgnoreCase = ignoreCase }.WithFormat(format));
    }

    private readonly FrozenDictionary<string, T> _nameToValueLookup;
    private readonly FrozenDictionary<T, string> _valueToNameLookup;
    private readonly ImmutableArray<string> _names; // names in the same order as Enum<T>.Values
    private readonly SpanAction<char, IntPtr> _toStringHelper;
#if NET9_0_OR_GREATER
    private readonly FrozenDictionary<string, T>.AlternateLookup<ReadOnlySpan<char>> _nameSpanToValueLookup;
#endif

    private readonly string _toStringSeparator;
    private readonly char _parseSeparator;
    private readonly bool _isSeparatorWhitespace;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumConverter{T}"/> class.
    /// </summary>
    /// <param name="buildOptionsAction">A delegate that builds the options used to customize the behavior of the converter.</param>
    public EnumConverter(Action<EnumConvertOptions> buildOptionsAction) : this(BuildOptions(buildOptionsAction)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumConverter{T}"/> class.
    /// </summary>
    /// <param name="options">The options to customize the behavior of the converter.</param>
    public EnumConverter(EnumConvertOptions options)
    {
        if (Enum<T>.IsFlagsEnum)
        {
            _toStringSeparator = options.Separator;
            _parseSeparator = options._separatorChar;
            _isSeparatorWhitespace = char.IsWhiteSpace(_parseSeparator);
        }
        else
        {
            _toStringSeparator = string.Empty;
        }

        var nameComparer = options.IgnoreCase ? StringComparer.OrdinalIgnoreCase : null;
        var nameToValueLookup = new Dictionary<string, T>(nameComparer);
        var valueToNameLookup = new Dictionary<T, string>();

        string[] names = new string[Enum<T>.Values.Length];

        foreach (var field in Enum<T>.GetFields())
        {
            string name = options.NameGetter.Invoke(field)?.Trim();

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Null or empty enumeration name.");

            if (Enum<T>.IsFlagsEnum && name.Contains(_parseSeparator))
                throw new ArgumentException($"Enumeration name '{name}' contains the parsing separator character '{_parseSeparator}'.");

            var value = (T)field.GetValue(null)!;

            if (UnderlyingOperations.TryParse(name, out T parsedValue) && !EqualityComparer<T>.Default.Equals(parsedValue, value))
            {
                throw new ArgumentException(
                    $"Numeric enumeration name '{name}' is not valid because it can be parsed as the value '{parsedValue}' " +
                    $"which does not match the actual value '{value}'.");
            }

            if (!nameToValueLookup.TryAdd(name, value))
                throw new ArgumentException($"Duplicate enumeration name '{name}'.");

            valueToNameLookup.TryAdd(value, name);

            int nameIndex = Enum<T>.GetFirstValueIndex(value);

            while (names[nameIndex] is not null)
                nameIndex++;

            names[nameIndex] = name;
        }

        _nameToValueLookup = nameToValueLookup.ToFrozenDictionary(nameComparer);
        _valueToNameLookup = valueToNameLookup.ToFrozenDictionary();
        _names = ImmutableCollectionsMarshal.AsImmutableArray(names);
        _toStringHelper = CreateToStringHelper(_toStringSeparator);

#if NET9_0_OR_GREATER
        _nameSpanToValueLookup = _nameToValueLookup.GetAlternateLookup<ReadOnlySpan<char>>();
#endif
    }

    /// <inheritdoc cref="EnumExtensions.GetName{T}(T)"/>
    public string GetName(T value)
    {
        if (!TryGetName(value, out string name))
        {
            [DoesNotReturn]
            static void Throw(T value) => throw new MissingMemberException($"An enumeration with the value '{value}' was not found.");
            Throw(value);
        }

        return name;
    }

    /// <inheritdoc cref="Enum{T}.GetValue(string, bool)"/>
    public T GetValue(string name)
    {
        if (!TryGetValue(name, out T value))
        {
            [DoesNotReturn]
            static void Throw(string name) => throw new MissingMemberException($"An enumeration with the name '{name}' was not found.");
            Throw(name);
        }

        return value;
    }

    /// <inheritdoc cref="Enum{T}.GetValue(string, bool)"/>
    public T GetValue(ReadOnlySpan<char> name)
    {
        if (!TryGetValue(name, out T value))
        {
            [DoesNotReturn]
            static void Throw(ReadOnlySpan<char> name)
            {
#if NET
                string message = $"An enumeration with the name '{name}' was not found.";
#else
                string message = $"An enumeration with the name '{name.ToString()}' was not found.";
#endif
                throw new MissingMemberException(message);
            }

            Throw(name);
        }

        return value;
    }

    /// <inheritdoc cref="Enum{T}.Parse(string, bool)"/>
    public T Parse(string s)
    {
        if (TryParse(s, out T value))
            return value;

        [DoesNotReturn]
        static void Throw() => throw new FormatException("Input string was not in a correct format.");
        Throw();
        return default;
    }

    /// <inheritdoc cref="Enum{T}.Parse(string, bool)"/>
    public T Parse(ReadOnlySpan<char> s)
    {
        if (TryParse(s, out T value))
            return value;

        [DoesNotReturn]
        static void Throw() => throw new FormatException("Input string was not in a correct format.");
        Throw();
        return default;
    }

    /// <summary>
    /// Gets a string representation of the specified enumeration value.
    /// </summary>
    /// <param name="value">The enumeration value.</param>
    public string ToString(T value) => ToString(value, SplitFlagsOptions.None);

    /// <summary>
    /// Gets a string representation of the specified enumeration value using the specified options to customize how flags are split.
    /// </summary>
    /// <param name="value">The enumeration value.</param>
    /// <param name="flagsOptions">Options to customize the behavior of the conversion if the value is a flags enumeration. Ignored if the value is not a flags
    /// enumeration.</param>
    [SkipLocalsInit]
    public unsafe string ToString(T value, SplitFlagsOptions flagsOptions)
    {
        flagsOptions.EnsureValid(nameof(flagsOptions));

        if (EqualityComparer<T>.Default.Equals(value, default))
            return Enum<T>.DefaultIndex >= 0 ? _names[Enum<T>.DefaultIndex] : "0";

        bool allMatchingFlags = flagsOptions.HasAllFlags(SplitFlagsOptions.AllMatchingFlags);

        if (!Enum<T>.IsFlagsEnum || !allMatchingFlags)
        {
            if (TryGetName(value, out string name))
                return name;

            if (!Enum<T>.IsFlagsEnum)
                return UnderlyingOperations.ToString(value);
        }

        // We have a flags enum that is not a simple value in the lookup or a default value.

        // If AllMatchingFlags is not specified then max possible number of matched flags is 64, otherwise it's the number of defined flags.
        // AllMatching flags will be rare and even more rare with > 64 flags so we stack alloc for the common case.

        const int MaxStackAllocLength = 64;
        bool doStackAlloc = Enum<T>.Values.Length <= MaxStackAllocLength || !allMatchingFlags;
        int[] rented = null;
        Span<int> foundItems = doStackAlloc ? stackalloc int[MaxStackAllocLength] : (rented = ArrayPool<int>.Shared.Rent(Enum<T>.Values.Length));
        string result;

        bool singleBitFlagsOnly = flagsOptions.HasAllFlags(SplitFlagsOptions.SingleBitFlagsOnly);
        EnumExtensions.SplitFlagsDescending(value, allMatchingFlags, singleBitFlagsOnly, foundItems, out int foundItemsCount, out T remainder);

        bool skipRemainder = EqualityComparer<T>.Default.Equals(remainder, default) || flagsOptions.HasAllFlags(SplitFlagsOptions.ExcludeRemainder);
        string remainderString = null;
        int resultLength;

        if (skipRemainder)
        {
            if (foundItemsCount is 0)
            {
                result = Enum<T>.DefaultIndex >= 0 ? _names[Enum<T>.DefaultIndex] : "0";
                goto Done;
            }

            resultLength = _toStringSeparator.Length * (foundItemsCount - 1);
        }
        else
        {
            flagsOptions.ThrowIfThrowOnRemainderSet(nameof(value));
            remainderString = UnderlyingOperations.ToString(remainder);

            if (foundItemsCount is 0)
            {
                result = remainderString;
                goto Done;
            }

            resultLength = (_toStringSeparator.Length * foundItemsCount) + remainderString.Length;
        }

        foundItems = foundItems[..foundItemsCount];

        foreach (int item in foundItems)
            resultLength += _names[item].Length;

        var toStringState = new ToStringState(foundItems, _names.AsSpan(), remainderString);
        result = StringMethods.Create(resultLength, (IntPtr)(&toStringState), _toStringHelper);

        Done:
        if (rented is not null) ArrayPool<int>.Shared.Return(rented);
        return result;
    }

    /// <inheritdoc cref="ToString(T)"/>
    [Obsolete("Use ToString(T) instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string AsString(T value) => ToString(value, SplitFlagsOptions.None);

    /// <inheritdoc cref="ToString(T, SplitFlagsOptions)"/>
    [Obsolete("Use ToString(T, SplitFlagsOptions) instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string AsString(T value, SplitFlagsOptions flagsOptions) => ToString(value, flagsOptions);

    private ref struct ToStringState(ReadOnlySpan<int> foundItems, ReadOnlySpan<string> names, string? remainderString)
    {
        public ReadOnlySpan<int> FoundItems = foundItems;
        public ReadOnlySpan<string> Names = names;
        public string? RemainderString = remainderString;
    }

    // Helper that creates the callback for creating the string result in ToString.
    // It optimizes for small separator lengths (and also the default value specifically) to ensure best performance for cases most affected.
    private static unsafe SpanAction<char, IntPtr> CreateToStringHelper(string toStringSeparator)
    {
        return toStringSeparator switch
        {
            [var c0] => (chars, state) =>
            {
                var stateValue = *(ToStringState*)state;
                var foundItems = stateValue.FoundItems;
                string remainderString = stateValue.RemainderString;
                var names = stateValue.Names;

                for (int i = foundItems.Length - 1; i > 0; i--)
                {
                    string name = names[foundItems[i]];
                    name.CopyTo(chars);
                    var nextChars = chars[(name.Length + 1)..];
                    chars[name.Length] = c0;
                    chars = nextChars;
                }

                Debug.Assert(foundItems.Length > 0, "Expected at least one found item.");
                string lastName = names[foundItems[0]];
                lastName.CopyTo(chars);
                if (remainderString is not null)
                {
                    chars = chars[lastName.Length..];
                    var nextChars = chars[1..];
                    chars[0] = c0;
                    chars = nextChars;
                    remainderString.CopyTo(chars);
                }
            },
            ", " => (chars, state) =>
            {
                var stateValue = *(ToStringState*)state;
                var foundItems = stateValue.FoundItems;
                string remainderString = stateValue.RemainderString;
                var names = stateValue.Names;

                for (int i = foundItems.Length - 1; i > 0; i--)
                {
                    string name = names[foundItems[i]];
                    name.CopyTo(chars);
                    var nextChars = chars[(name.Length + 2)..];
                    chars[name.Length] = ',';
                    chars[name.Length + 1] = ' ';
                    chars = nextChars;
                }

                Debug.Assert(foundItems.Length > 0, "Expected at least one found item.");
                string lastName = names[foundItems[0]];
                lastName.CopyTo(chars);
                if (remainderString is not null)
                {
                    chars = chars[lastName.Length..];
                    var nextChars = chars[2..];
                    chars[0] = ',';
                    chars[1] = ' ';
                    chars = nextChars;
                    remainderString.CopyTo(chars);
                }
            },
            [var c0, var c1] => (chars, state) =>
            {
                var stateValue = *(ToStringState*)state;
                var foundItems = stateValue.FoundItems;
                string remainderString = stateValue.RemainderString;
                var names = stateValue.Names;

                for (int i = foundItems.Length - 1; i > 0; i--)
                {
                    string name = names[foundItems[i]];
                    name.CopyTo(chars);
                    var nextChars = chars[(name.Length + 2)..];
                    chars[name.Length] = c0;
                    chars[name.Length + 1] = c1;
                    chars = nextChars;
                }

                Debug.Assert(foundItems.Length > 0, "Expected at least one found item.");
                string lastName = names[foundItems[0]];
                lastName.CopyTo(chars);
                if (remainderString is not null)
                {
                    chars = chars[lastName.Length..];
                    var nextChars = chars[2..];
                    chars[0] = c0;
                    chars[1] = c1;
                    chars = nextChars;
                    remainderString.CopyTo(chars);
                }
            },
            _ => (chars, state) =>
            {
                var stateValue = *(ToStringState*)state;
                var foundItems = stateValue.FoundItems;
                string remainderString = stateValue.RemainderString;
                var names = stateValue.Names;
                var toStringSeparatorSp = toStringSeparator.AsSpan();

                for (int i = foundItems.Length - 1; i > 0; i--)
                {
                    string name = names[foundItems[i]];
                    name.CopyTo(chars);
                    chars = chars[name.Length..];
                    toStringSeparatorSp.CopyTo(chars);
                    chars = chars[toStringSeparatorSp.Length..];
                }

                Debug.Assert(foundItems.Length > 0, "Expected at least one found item.");
                string lastName = names[foundItems[0]];
                lastName.CopyTo(chars);
                if (remainderString is not null)
                {
                    chars = chars[lastName.Length..];
                    toStringSeparatorSp.CopyTo(chars);
                    chars = chars[toStringSeparatorSp.Length..];
                    remainderString?.CopyTo(chars);
                }
            },
        };
    }

    /// <inheritdoc cref="Enum{T}.TryGetValue(string, out T)" />
    public bool TryGetValue(string name, out T value) => _nameToValueLookup.TryGetValue(name, out value);

    /// <inheritdoc cref="Enum{T}.TryGetValue(string, out T)" />
    public bool TryGetValue(ReadOnlySpan<char> name, out T value)
    {
#if NET9_0_OR_GREATER
        return _nameSpanToValueLookup.TryGetValue(name, out value);
#else
        return _nameToValueLookup.TryGetValue(name.ToString(), out value);
#endif
    }

    /// <inheritdoc cref="EnumExtensions.TryGetName{T}(T, out string?)"/>
    public bool TryGetName(T value, [NotNullWhen(true)] out string? name) => _valueToNameLookup.TryGetValue(value, out name);

    /// <inheritdoc cref="Enum{T}.TryParse(string, out T)"/>
    public bool TryParse(string s, out T value)
    {
        _ = s.Length;
        return TryParseImpl(s, s.AsSpan(), out value);
    }

    /// <inheritdoc cref="Enum{T}.TryParse(string, out T)"/>
    public bool TryParse(ReadOnlySpan<char> s, out T value)
    {
        return TryParseImpl(null, s, out value);
    }

    private bool TryParseImpl(string? s, ReadOnlySpan<char> sp, out T value)
    {
        if (!Enum<T>.IsFlagsEnum)
        {
#if NET9_0_OR_GREATER
            return TryGetNamedOrNumericValueSpan(sp.Trim(), out value);
#else
            if (s is not null)
                return TryGetNamedOrNumericValue(s.Trim(), out value);

            return TryGetNamedOrNumericValueSpan(sp.Trim(), out value);
#endif
        }

        value = default;
        int start = 0;

        while (true)
        {
            if (start == sp.Length)
                return false;

            if (char.IsWhiteSpace(sp[start]))
                start++;
            else
                break;
        }

        sp = sp[start..];

        while (true)
        {
            int separator = sp.IndexOf(_parseSeparator);
            int exclusiveEnd = separator < 0 ? sp.Length : separator;

            if (exclusiveEnd is 0)
                return false;

            while (char.IsWhiteSpace(sp[exclusiveEnd - 1]))
                exclusiveEnd--;

            var part = sp[..exclusiveEnd];

#if NET9_0_OR_GREATER
            if (!TryGetNamedOrNumericValueSpan(part, out T partValue))
                return false;
#else
            T partValue;

            if (s is not null && part.Length == s.Length)
            {
                if (!TryGetNamedOrNumericValue(s, out partValue))
                    return false;
            }
            else if (!TryGetNamedOrNumericValueSpan(part, out partValue))
            {
                return false;
            }
#endif
            value = value.SetFlags(partValue);

            if (separator < 0)
                return true;

            start = separator + 1;

            while (true)
            {
                if (start == sp.Length)
                    return _isSeparatorWhitespace;

                if (char.IsWhiteSpace(sp[start]))
                {
                    start++;
                }
                else
                {
                    sp = sp[start..];
                    break;
                }
            }
        }

        // Parsing helpers:

#if NET9_0_OR_GREATER
        bool TryGetNamedOrNumericValueSpan(ReadOnlySpan<char> s, out T value)
        {
            if (_nameSpanToValueLookup.TryGetValue(s, out value))
                return true;

            return UnderlyingOperations.TryParse(s, out value);
        }
#else
        bool TryGetNamedOrNumericValue(string s, out T value)
        {
            if (_nameToValueLookup.TryGetValue(s, out value))
                return true;

            return UnderlyingOperations.TryParse(s, out value);
        }

        bool TryGetNamedOrNumericValueSpan(ReadOnlySpan<char> s, out T value)
        {
            string str = s.ToString();

            if (_nameToValueLookup.TryGetValue(str, out value))
                return true;

            return UnderlyingOperations.TryParse(str, out value);
        }
#endif
    }

    private static EnumConvertOptions BuildOptions(Action<EnumConvertOptions> buildOptionsAction)
    {
        var options = new EnumConvertOptions();
        buildOptionsAction(options);
        return options;
    }

    /// <summary>
    /// Caches the shared converters that use the <see cref="EnumFormat.DisplayName"/> format. Display names are resolved when a converter is created and are
    /// localized using the current UI culture if any field's <see cref="DisplayAttribute"/> specifies a resource type, so in that case converters are cached
    /// per culture. Cultures are keyed by name rather than by reference because equal cultures can have distinct instances (e.g. ASP.NET Core request
    /// localization hands out read-only clones), and localized resources are only ever resolved by name.
    /// </summary>
    private static class DisplayNameConverters
    {
        private static readonly bool _isCultureSensitive =
            Enum<T>.GetFields().Any(f => f.GetCustomAttribute<DisplayAttribute>()?.ResourceType is not null);

        private static ConverterPair? _invariantPair;
        private static ConverterPair? _lastPair;
        private static ConcurrentDictionary<string, ConverterPair>? _pairsByCultureName;

        public static EnumConverter<T> Get(CultureInfo? culture, bool ignoreCase)
        {
            if (!_isCultureSensitive)
                return (_invariantPair ??= new ConverterPair(null)).Get(ignoreCase);

            culture ??= CultureInfo.CurrentUICulture;
            var pair = _lastPair;

            if (pair is null || pair.Culture!.Name != culture.Name)
            {
                var pairs = _pairsByCultureName ?? InitPairs();

                if (!pairs.TryGetValue(culture.Name, out pair))
                    pair = pairs.GetOrAdd(culture.Name, new ConverterPair(culture));

                _lastPair = pair;
            }

            return pair.Get(ignoreCase);
        }

        private static ConcurrentDictionary<string, ConverterPair> InitPairs()
        {
            Interlocked.CompareExchange(ref _pairsByCultureName, new ConcurrentDictionary<string, ConverterPair>(), null);
            return _pairsByCultureName;
        }

        private sealed class ConverterPair(CultureInfo? culture)
        {
            private EnumConverter<T>? _converter;
            private EnumConverter<T>? _converterIgnoreCase;

            public CultureInfo? Culture { get; } = culture;

            public EnumConverter<T> Get(bool ignoreCase)
            {
                return ignoreCase ?
                    (_converterIgnoreCase ??= Create(ignoreCase)) :
                    (_converter ??= Create(ignoreCase));
            }

            private EnumConverter<T> Create(bool ignoreCase)
            {
                var options = new EnumConvertOptions() { IgnoreCase = ignoreCase }.WithFormat(EnumFormat.DisplayName);

                // DisplayAttribute resolves localized names using the current UI culture and has no way to specify a different culture, so the current UI
                // culture is temporarily switched if a different culture was requested.

                var currentCulture = CultureInfo.CurrentUICulture;

                if (Culture is null || Culture.Name == currentCulture.Name)
                    return new EnumConverter<T>(options);

                CultureInfo.CurrentUICulture = Culture;

                try
                {
                    return new EnumConverter<T>(options);
                }
                finally
                {
                    CultureInfo.CurrentUICulture = currentCulture;
                }
            }
        }
    }
}
