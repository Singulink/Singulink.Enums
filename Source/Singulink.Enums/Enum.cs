using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

#pragma warning disable CA1716 // Identifiers should not match keywords

namespace Singulink.Enums
{
    /// <summary>
    /// Provides methods and properties for getting enumeration names, values and other useful information.
    /// </summary>
    public static class Enum<T> where T : unmanaged, Enum
    {
        private static readonly bool _isFlagsEnum = typeof(T).GetCustomAttribute<FlagsAttribute>() != null;
        private static readonly IComparer<T> _valueComparer = _isFlagsEnum ? new FlagsValueComparer() : Comparer<T>.Default;

        private static ImmutableArray<T> _values;
        private static ImmutableArray<string> _names;

        /// <summary>
        /// Gets a value indicating whether this enumeration is a flags enumeration (i.e. has <see cref="FlagsAttribute"/> applied to it).
        /// </summary>
        public static bool IsFlagsEnum => _isFlagsEnum;

        /// <summary>
        /// Gets all the defined names for the enumeration.
        /// </summary>
        /// <remarks>
        /// <para>The names are ordered by their values and their indexes match their value's index in the <see cref="Values"/> collection. If this is a flags
        /// enumeration then the values are ordered by their unsigned equivalent values to ensure they are ordered based on the bits that are set, so negative
        /// values will be at the end.</para>
        /// </remarks>
        public static ImmutableArray<string> Names {
            get {
                InitializeFields();
                return _names;
            }
        }

        /// <summary>
        /// Gets all the defined values for the enumeration.
        /// </summary>
        /// <remarks>
        /// <para>The values are ordered and their indexes match their name's index in the <see cref="Names"/> collection. If this is a flags enumeration then
        /// the values are ordered by their unsigned equivalent values to ensure they are ordered based on the bits that are set, so negative values will be at
        /// the end.</para>
        /// </remarks>
        public static ImmutableArray<T> Values {
            get {
                InitializeFields();
                return _values;
            }
        }

        /// <summary>
        /// Gets the enumeration members. This is a slow reflection-based operation that does not cache the results.
        /// </summary>
        public static IEnumerable<EnumMember<T>> GetMembers() => typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public).Select(f => new EnumMember<T>(f));

        internal static IComparer<T> ValueComparer => _valueComparer;

        /// <summary>
        /// Gets the first enumeration field name with the given value.
        /// </summary>
        /// <remarks>
        /// <para>The name is retrieved in O(log n) time. Use <see cref="EnumParser{T}"/> for O(1) parsing and string conversion.</para>
        /// </remarks>
        public static bool TryGetName(T value, [NotNullWhen(true)] out string? name)
        {
            int index = Values.BinarySearch(value, _valueComparer);

            if (index < 0) {
                name = null;
                return false;
            }

            name = _names[index];
            return true;
        }

        /// <summary>
        /// Gets the first enumeration field name with the given value.
        /// </summary>
        /// <remarks>
        /// <para>The name is retrieved in O(log n) time. Use <see cref="EnumParser{T}"/> for O(1) parsing and string conversion.</para>
        /// </remarks>
        public static string GetName(T value)
        {
            if (!TryGetName(value, out string? name)) {
                var underlyingType = Enum.GetUnderlyingType(typeof(T));
                object underlyingValue = Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
                throw new MissingMemberException($"An enumeration member with the value '{underlyingValue}' was not found.");
            }

            return name;
        }

        /// <summary>
        /// Gets the first enumeration field value with the given name.
        /// </summary>
        /// <remarks>
        /// <para>The value is retrieved in O(n) time. Use <see cref="EnumParser{T}"/> for O(1) parsing and string conversion.</para>
        /// </remarks>
        public static bool TryGetValue(string name, out T value, bool ignoreCase = false)
        {
            InitializeFields();

            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            for (int i = 0; i < _names.Length; i++) {
                if (string.Equals(_names[i], name, comparison)) {
                    value = _values![i];
                    return true;
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the first enumeration field value with the given name.
        /// </summary>
        /// <remarks>
        /// <para>The value is retrieved in O(n) time. Use <see cref="EnumParser{T}"/> for O(1) parsing and string conversion.</para>
        /// </remarks>
        public static T GetValue(string name, bool ignoreCase = false)
        {
            if (!TryGetValue(name, out T value, ignoreCase))
                throw new MissingMemberException($"An enumeration member with the name '{name}' was not found.");

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitializeFields()
        {
            if (_names.IsDefault)
                Initialize();

            static void Initialize()
            {
                var members = GetMembers()
                    .Select(f => (f.Name, f.Value))
                    .OrderBy(m => m.Value, _valueComparer)
                    .ToArray();

                _values = members.Select(m => m.Value).ToImmutableArray();
                _names = members.Select(m => m.Name).ToImmutableArray();
            }
        }

        /// <summary>
        /// Orders flags in reverse order of highest bit set regardless of whether the underlying value is signed or unsigned.
        /// </summary>
        private sealed class FlagsValueComparer : IComparer<T>
        {
            public int Compare(T x, T y)
            {
                int xc = Comparer<T>.Default.Compare(x, default);
                int yc = Comparer<T>.Default.Compare(y, default);

                if (xc >= 0) {
                    if (yc >= 0)
                        return Comparer<T>.Default.Compare(x, y);

                    return -1;
                }

                if (yc >= 0)
                    return 1;

                // Both are negative:
                return Comparer<T>.Default.Compare(x, y);
            }
        }
    }
}
