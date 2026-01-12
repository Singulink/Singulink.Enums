using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Singulink.Enums;

/// <summary>
/// Provides methods and properties for getting enumeration names, values and other useful information.
/// </summary>
public static class Enum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>
    where T : unmanaged, Enum
{
    private static readonly bool _isFlagsEnum = typeof(T).GetCustomAttribute<FlagsAttribute>() is not null;
    private static readonly (ImmutableArray<T> Values, ImmutableArray<string> Names, ImmutableArray<int> FirstItemWithValueLookup) _info = InitInfo();
    private static readonly int _valuesLength = _info.Values.Length;
    private static readonly int _defaultIndex = Values.IndexOf(default);

    /// <summary>
    /// Gets a value indicating whether this enumeration is a flags enumeration (i.e. has <see cref="FlagsAttribute"/> applied to it).
    /// </summary>
    public static bool IsFlagsEnum => _isFlagsEnum;

    /// <summary>
    /// Gets all the defined names for the enumeration, ordered by their value.
    /// </summary>
    /// <remarks>
    /// The index of each value in <see cref="Values"/> matches the index of its name in <see cref="Names"/>. For flags enumerations, the values are
    /// ordered by the highest bits set in ascending order, otherwise they are ordered by their underlying value in ascending order.
    /// </remarks>
    public static ImmutableArray<string> Names => _info.Names;

    /// <summary>
    /// Gets all the defined values for the enumeration.
    /// </summary>
    /// <remarks>
    /// The index of each value in <see cref="Values"/> matches the index of its name in <see cref="Names"/>. For flags enumeration, the values are ordered by
    /// the highest bits set in ascending order, otherwise they are ordered by their underlying value in ascending order.
    /// </remarks>
    public static ImmutableArray<T> Values => _info.Values;

    internal static int DefaultIndex => _defaultIndex;

    /// <summary>
    /// Gets the enumeration fields that define its names and values. This is a slow reflection-based operation that is not cached.
    /// </summary>
    public static IEnumerable<FieldInfo> GetFields() => typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public).Where(f => f.FieldType == typeof(T));

    /// <summary>
    /// Gets the enumeration value with the given name.
    /// </summary>
    /// <param name="name">The name of the enumeration value.</param>
    /// <param name="ignoreCase"><see langword="true"/> for case-insensitive or <see langword="false"/> for case-sensitive parsing.</param>
    /// <exception cref="MissingMemberException">An enumeration with the specified name was not found.</exception>
    public static T GetValue(string name, bool ignoreCase = false)
    {
        return ignoreCase ? EnumConverter<T>.DefaultIgnoreCase.GetValue(name) : EnumConverter<T>.Default.GetValue(name);
    }

    /// <inheritdoc cref="GetValue(string, bool)" />
    public static T GetValue(ReadOnlySpan<char> name, bool ignoreCase = false)
    {
        return ignoreCase ? EnumConverter<T>.DefaultIgnoreCase.GetValue(name) : EnumConverter<T>.Default.GetValue(name);
    }

    /// <summary>
    /// Gets the enumeration value parsed from the specified string.
    /// </summary>
    /// <param name="s">The string to be parsed.</param>
    /// <param name="ignoreCase"><see langword="true"/> for case-insensitive or <see langword="false"/> for case-sensitive parsing.</param>
    /// <exception cref="FormatException">Input string was not in a correct format.</exception>
    public static T Parse(string s, bool ignoreCase = false)
    {
        return ignoreCase ? EnumConverter<T>.DefaultIgnoreCase.Parse(s) : EnumConverter<T>.Default.Parse(s);
    }

    /// <inheritdoc cref="Parse(string, bool)"/>
    public static T Parse(ReadOnlySpan<char> s, bool ignoreCase = false)
    {
        return ignoreCase ? EnumConverter<T>.DefaultIgnoreCase.Parse(s) : EnumConverter<T>.Default.Parse(s);
    }

    /// <inheritdoc cref="TryGetValue(string, bool, out T)"/>
    public static bool TryGetValue(string name, out T value) => EnumConverter<T>.Default.TryGetValue(name, out value);

    /// <inheritdoc cref="TryGetValue(string, bool, out T)"/>
    public static bool TryGetValue(ReadOnlySpan<char> name, out T value) => EnumConverter<T>.Default.TryGetValue(name, out value);

    /// <summary>
    /// Gets the enumeration value with the given name.
    /// </summary>
    /// <param name="name">The name of the enumeration value.</param>
    /// <param name="ignoreCase"><see langword="true"/> for case-insensitive or <see langword="false"/> for case-sensitive matching.</param>
    /// <param name="value">Contains the matched value when the method returns if the value was found, otherwise the default value of <typeparamref
    /// name="T"/>.</param>
    /// <returns><see langword="true"/> if the value was found, otherwise <see langword="false"/>.</returns>
    public static bool TryGetValue(string name, bool ignoreCase, out T value)
    {
        return ignoreCase ? EnumConverter<T>.DefaultIgnoreCase.TryGetValue(name, out value) : EnumConverter<T>.Default.TryGetValue(name, out value);
    }

    /// <inheritdoc cref="TryGetValue(string, bool, out T)"/>
    public static bool TryGetValue(ReadOnlySpan<char> name, bool ignoreCase, out T value)
    {
        return ignoreCase ? EnumConverter<T>.DefaultIgnoreCase.TryGetValue(name, out value) : EnumConverter<T>.Default.TryGetValue(name, out value);
    }

    /// <inheritdoc cref="TryParse(string, bool, out T)"/>
    public static bool TryParse(string s, out T value) => EnumConverter<T>.Default.TryParse(s, out value);

    /// <inheritdoc cref="TryParse(string, bool, out T)"/>
    public static bool TryParse(ReadOnlySpan<char> s, out T value) => EnumConverter<T>.Default.TryParse(s, out value);

    /// <summary>
    /// Gets the enumeration value parsed from the specified string.
    /// </summary>
    /// <param name="s">The string to be parsed.</param>
    /// <param name="ignoreCase"><see langword="true"/> for case-insensitive or <see langword="false"/> for case-sensitive parsing.</param>
    /// <param name="value">Contains the resulting value when the method returns if parsing was successful, otherwise the default value of <typeparamref
    /// name="T"/>.</param>
    /// <returns><see langword="true"/> if parsing was successful, otherwise <see langword="false"/>.</returns>
    public static bool TryParse(string s, bool ignoreCase, out T value)
    {
        return ignoreCase ? EnumConverter<T>.DefaultIgnoreCase.TryParse(s, out value) : EnumConverter<T>.Default.TryParse(s, out value);
    }

    /// <inheritdoc cref="TryParse(string, bool, out T)"/>
    public static bool TryParse(ReadOnlySpan<char> s, bool ignoreCase, out T value)
    {
        return ignoreCase ? EnumConverter<T>.DefaultIgnoreCase.TryParse(s, out value) : EnumConverter<T>.Default.TryParse(s, out value);
    }

    internal static bool ContainsValue(T value)
    {
        if (_valuesLength <= 20)
            return _info.Values.Contains(value);

        return BinarySearchValues(value) >= 0;
    }

    internal static int GetFirstValueIndex(T value)
    {
        if (_valuesLength <= 20)
            return _info.Values.IndexOf(value);

        int idx = BinarySearchValues(value);

        // If there are duplicate values, we want to return the first one.
        if (!_info.FirstItemWithValueLookup.IsDefault)
            idx = _info.FirstItemWithValueLookup[idx];

        return idx;
    }

    internal static void EnsureIsFlagsEnum()
    {
        if (!IsFlagsEnum)
        {
            static void Throw() => throw new InvalidOperationException($"Type '{typeof(T)}' is not a flags enumeration.");
            Throw();
        }
    }

    private static unsafe int BinarySearchValues(T value)
    {
        if (!_isFlagsEnum)
            return Values.BinarySearch(value);

        if (sizeof(T) is 1)
            return ValuesAs<byte>().BinarySearch(UnsafeMethods.BitCast<T, byte>(value));
        else if (sizeof(T) is 2)
            return ValuesAs<ushort>().BinarySearch(UnsafeMethods.BitCast<T, ushort>(value));
        else if (sizeof(T) is 4)
            return ValuesAs<uint>().BinarySearch(UnsafeMethods.BitCast<T, uint>(value));
        else if (sizeof(T) is 8)
            return ValuesAs<ulong>().BinarySearch(UnsafeMethods.BitCast<T, ulong>(value));

        throw new NotSupportedException();

        static ImmutableArray<TValue> ValuesAs<TValue>() => Unsafe.As<ImmutableArray<T>, ImmutableArray<TValue>>(ref Unsafe.AsRef(in _info.Values));
    }

    private static (ImmutableArray<T> Values, ImmutableArray<string> Names, ImmutableArray<int> FirstItemWithValueLookup) InitInfo()
    {
        var members = GetFields().Select(f => (f.Name, Value: (T)f.GetValue(null)!));

        // For flags enums we order by unsigned value to ensure that bits are ordered correctly, but for non-flags enums we order by signed value.
        if (_isFlagsEnum)
            members = members.OrderBy(m => new UnsignedComparable(m.Value));
        else
            members = members.OrderBy(m => m.Value);

        var membersList = members.ToArray();

        var values = membersList.Select(m => m.Value).ToImmutableArray();
        var names = membersList.Select(m => m.Name).ToImmutableArray();

        // We only need to build the lookup if there are duplicate values - we will compare to default to determine if we need to worry about this, so only
        // create it if it's actually needed - this is not the maximally optimal solution for looking up first index by value, but it's good enough & allows us
        // to easily avoid the overhead when not needed, and only requires 1 side-buffer.
        // That is, this solution is simple, but still keeps the common case as fast as possible, while providing correct behaviour and decent performance in
        // at least all cases.
        ImmutableArray<int> firstItemWithValueLookup = default;
        if (values.Distinct().Count() != values.Length)
        {
            var builder = ImmutableArray.CreateBuilder<int>(values.Length);
            builder.Add(0);

            int lastIndex = 0;
            var lastValue = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(values[i], lastValue))
                {
                    lastValue = values[i];
                    lastIndex = i;
                }

                builder.Add(lastIndex);
            }

            firstItemWithValueLookup = builder.DrainToImmutable();
        }

        return (values, names, firstItemWithValueLookup);
    }

    private unsafe readonly struct UnsignedComparable(T value) : IComparable<UnsignedComparable>
    {
        private readonly T _value = value;

        public int CompareTo(UnsignedComparable other)
        {
            if (sizeof(T) is 1)
                return UnsafeMethods.BitCast<T, byte>(_value).CompareTo(UnsafeMethods.BitCast<T, byte>(other._value));
            else if (sizeof(T) is 2)
                return UnsafeMethods.BitCast<T, ushort>(_value).CompareTo(UnsafeMethods.BitCast<T, ushort>(other._value));
            else if (sizeof(T) is 4)
                return UnsafeMethods.BitCast<T, uint>(_value).CompareTo(UnsafeMethods.BitCast<T, uint>(other._value));
            else if (sizeof(T) is 8)
                return UnsafeMethods.BitCast<T, ulong>(_value).CompareTo(UnsafeMethods.BitCast<T, ulong>(other._value));

            throw new NotSupportedException();
        }
    }
}
