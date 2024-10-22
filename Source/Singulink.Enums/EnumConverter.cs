using System.Buffers;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

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

    /// <summary>
    /// Gets the default case-sensitive enumeration converter.
    /// </summary>
    public static EnumConverter<T> Default => _default ??= new EnumConverter<T>(new EnumConvertOptions());

    /// <summary>
    /// Gets the default case-insensitive enumeration converter.
    /// </summary>
    public static EnumConverter<T> DefaultIgnoreCase => _defaultIgnoreCase ??= new EnumConverter<T>(new EnumConvertOptions() { IgnoreCase = true });

    private readonly FrozenDictionary<string, T> _nameToValueLookup;
    private readonly FrozenDictionary<T, string> _valueToNameLookup;
    private readonly ImmutableArray<string> _names; // names in the same order as Enum<T>.Values

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
                throw new ArgumentException($"Enumeration name '{name}' contains the separator character.");

            var value = (T)field.GetValue(null)!;

#if NET8_0_OR_GREATER
            if (!nameToValueLookup.TryAdd(name, value))
                throw new ArgumentException($"Duplicate enumeration name '{name}'.");

            valueToNameLookup.TryAdd(value, name);
#else
            if (nameToValueLookup.ContainsKey(name))
                throw new ArgumentException($"Duplicate enumeration name '{name}'.");

            nameToValueLookup.Add(name, value);

            if (!valueToNameLookup.ContainsKey(value))
                valueToNameLookup.Add(value, name);
#endif
            names[Enum<T>.BinarySearchValues(value)] = name;
        }

        _nameToValueLookup = nameToValueLookup.ToFrozenDictionary(nameComparer);
        _valueToNameLookup = valueToNameLookup.ToFrozenDictionary();
        _names = Unsafe.As<string[], ImmutableArray<string>>(ref names);
    }

    /// <inheritdoc cref="EnumExtensions.GetName{T}(T)"/>
    public string GetName(T value)
    {
        if (!TryGetName(value, out string name))
            throw new MissingMemberException($"An enumeration with the value '{value}' was not found.");

        return name;
    }

    /// <inheritdoc cref="Enum{T}.GetValue(string, bool)"/>
    public T GetValue(string name)
    {
        if (!TryGetValue(name, out T value))
            throw new MissingMemberException($"An enumeration with the name '{name}' was not found.");

        return value;
    }

    /// <inheritdoc cref="Enum{T}.Parse(string, bool)"/>
    public T Parse(string s)
    {
        if (TryParse(s, out T value))
            return value;

        throw new FormatException("Input string was not in a correct format.");
    }

    /// <inheritdoc cref="EnumExtensions.AsString{T}(T)"/>
    public string AsString(T value) => AsString(value, SplitFlagsOptions.None);

    /// <inheritdoc cref="EnumExtensions.AsString{T}(T, SplitFlagsOptions)"/>
    [SkipLocalsInit]
    public unsafe string AsString(T value, SplitFlagsOptions flagsOptions)
    {
        if (Enum<T>.IsFlagsEnum && !flagsOptions.IsValid())
            throw new ArgumentException("Value of options flags is invalid.", nameof(flagsOptions));

        if (!Enum<T>.IsFlagsEnum || !flagsOptions.HasAllFlags(SplitFlagsOptions.AllMatchingFlags))
        {
            if (TryGetName(value, out string name))
                return name;

            if (EqualityComparer<T>.Default.Equals(value, default))
                return "0";

            if (!Enum<T>.IsFlagsEnum)
                return UnderlyingOperations.ToString(value);
        }

        // We have a flags enum that is not a simple value in the lookup or a default value.

        bool doStackAlloc = Enum<T>.Values.Length <= 79 || !flagsOptions.HasAllFlags(SplitFlagsOptions.AllMatchingFlags);

        int[] rented = null;

        // Needs one extra slot for possible remainder
        Span<int> foundItems = doStackAlloc ? stackalloc int[80] : (rented = ArrayPool<int>.Shared.Rent(Enum<T>.Values.Length + 1));
        EnumExtensions.SplitFlagsDescending(value, flagsOptions, foundItems, out int foundItemsCount, out T remainder);
        foundItems = foundItems[..foundItemsCount];

        int resultLength = 0;

        foreach (int item in foundItems)
            resultLength += _names[item].Length;

        bool skipRemainder = EqualityComparer<T>.Default.Equals(remainder, default) || flagsOptions.HasAllFlags(SplitFlagsOptions.ExcludeRemainder);
        string remainderString = null;

        if (skipRemainder)
        {
            resultLength += _toStringSeparator.Length * (foundItemsCount - 1);
        }
        else
        {
            remainderString = UnderlyingOperations.ToString(remainder);
            resultLength += remainderString.Length + (_toStringSeparator.Length * foundItemsCount);
        }

        return StringMethods.Create(resultLength, (foundItemsPtr: (nint)(&foundItems), remainderString), (chars, state) => {
            var foundItems = *(ReadOnlySpan<int>*)state.foundItemsPtr;
            string remainderString = state.remainderString;

            for (int i = foundItems.Length - 1; i >= 0; i--)
            {
                int item = foundItems[i];
                var name = _names[item].AsSpan();
                name.CopyTo(chars);
                chars = chars[name.Length..];

                if (i < chars.Length || remainderString is not null)
                {
                    _toStringSeparator.AsSpan().CopyTo(chars);
                    chars = chars[_toStringSeparator.Length..];
                }
            }

            remainderString?.AsSpan().CopyTo(chars);
        });
    }

    /// <inheritdoc cref="Enum{T}.TryGetValue(string, out T)" />
    public bool TryGetValue(string name, out T value) => _nameToValueLookup.TryGetValue(name, out value);

    /// <inheritdoc cref="EnumExtensions.TryGetName{T}(T, out string?)"/>
    public bool TryGetName(T value, [NotNullWhen(true)] out string? name) => _valueToNameLookup.TryGetValue(value, out name);

    /// <inheritdoc cref="Enum{T}.TryParse(string, out T)"/>
    public bool TryParse(string s, out T value)
    {
        if (!Enum<T>.IsFlagsEnum)
            return TryGetNamedOrNumericValue(s.Trim(), out value);

        value = default;
        int start = 0;

        while (true)
        {
            if (start == s.Length)
                return false;

            if (char.IsWhiteSpace(s[start]))
                start++;
            else
                break;
        }

        while (true)
        {
            int separator = s.IndexOf(_parseSeparator, start);
            int exclusiveEnd = separator < 0 ? s.Length : separator;

            if (exclusiveEnd == start)
                return false;

            while (char.IsWhiteSpace(s[exclusiveEnd - 1]))
                exclusiveEnd--;

            string part = s[start..exclusiveEnd];

            if (!TryGetNamedOrNumericValue(part, out T partValue))
                return false;

            value = value.SetFlags(partValue);

            if (separator < 0)
                return true;

            start = separator + 1;

            while (true)
            {
                if (start == s.Length)
                    return _isSeparatorWhitespace;

                if (char.IsWhiteSpace(s[start]))
                    start++;
                else
                    break;
            }
        }

        bool TryGetNamedOrNumericValue(string s, out T value)
        {
            if (_nameToValueLookup.TryGetValue(s, out value))
                return true;

            return UnderlyingOperations.TryParse(s, out value);
        }
    }

    private static EnumConvertOptions BuildOptions(Action<EnumConvertOptions> buildOptionsAction)
    {
        var options = new EnumConvertOptions();
        buildOptionsAction(options);
        return options;
    }
}