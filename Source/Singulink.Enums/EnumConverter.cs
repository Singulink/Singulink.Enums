using System.Buffers;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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
            int nameIndex = Enum<T>.GetFirstValueIndex(value);

            while (names[nameIndex] is not null)
                nameIndex++;

            names[nameIndex] = name;
        }

        _nameToValueLookup = nameToValueLookup.ToFrozenDictionary(nameComparer);
        _valueToNameLookup = valueToNameLookup.ToFrozenDictionary();
        _names = ImmutableCollectionsMarshal.AsImmutableArray(names);
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

        EnumExtensions.SplitFlagsDescending(value, allMatchingFlags, foundItems, out int foundItemsCount, out T remainder);

        int resultLength;

        bool skipRemainder = EqualityComparer<T>.Default.Equals(remainder, default) || flagsOptions.HasAllFlags(SplitFlagsOptions.ExcludeRemainder);
        string remainderString = null;

        if (skipRemainder)
        {
            if (foundItemsCount is 0)
                return Enum<T>.DefaultIndex >= 0 ? _names[Enum<T>.DefaultIndex] : "0";

            resultLength = _toStringSeparator.Length * (foundItemsCount - 1);
        }
        else
        {
            flagsOptions.ThrowIfThrowOnRemainderSet(nameof(value));
            remainderString = UnderlyingOperations.ToString(remainder);

            if (foundItemsCount is 0)
                return remainderString;

            resultLength = (_toStringSeparator.Length * foundItemsCount) + remainderString.Length;
        }

        foundItems = foundItems[..foundItemsCount];

        foreach (int item in foundItems)
            resultLength += _names[item].Length;

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
