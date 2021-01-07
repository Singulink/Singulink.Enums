using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Singulink.Enums
{
    /// <summary>
    /// Provides enumeration string conversion functionality.
    /// </summary>
    public sealed class EnumParser<T> where T : unmanaged, Enum
    {
        /// <summary>
        /// Gets the default enumeration parser.
        /// </summary>
        public static EnumParser<T> Default { get; } = new EnumParser<T>();

        private readonly Dictionary<string, T> _valueLookup;
        private readonly Dictionary<T, string> _nameLookup;

        private readonly string _toStringSeparator;
        private readonly char[] _parseSeparatorArray;
        private readonly Type _underlyingType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumParser{T}"/> class.
        /// </summary>
        /// <param name="customNameGetter">Optional function to customize enumeration member names.</param>
        /// <param name="separator">A string containing a separator character with an optional leading and trailing space. This parameter is required for
        /// flags enumerations and ignored for regular enumerations.</param>
        /// <param name="caseSensitive">True for case-sensitive names, otherwise false.</param>
        public EnumParser(Func<EnumMember<T>, string>? customNameGetter = null, string? separator = ", ", bool caseSensitive = true)
        {
            customNameGetter ??= m => m.Name;

            if (Enum<T>.IsFlagsEnum) {
                _toStringSeparator = separator ?? throw new ArgumentNullException(nameof(separator), "Separator is required for flags enumerations.");
                _parseSeparatorArray = new[] { GetSeparatorChar(separator) };

                if (_parseSeparatorArray[0] == '-')
                    throw new ArgumentException("Hyphen is not allowed as a separator character.", nameof(separator));
            }
            else {
                _toStringSeparator = string.Empty;
                _parseSeparatorArray = Array.Empty<char>();
            }

            _underlyingType = Enum.GetUnderlyingType(typeof(T));

            _valueLookup = new Dictionary<string, T>(caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
            _nameLookup = new Dictionary<T, string>();

            foreach (var member in Enum<T>.GetMembers()) {
                string name = customNameGetter.Invoke(member)?.Trim();

                if (string.IsNullOrEmpty(name))
                    throw new ArgumentException("Null or empty enumeration member name.");

                var value = member.Value;

                if (Enum<T>.IsFlagsEnum && name.Contains(_parseSeparatorArray[0]))
                    throw new ArgumentException($"Enumeration member name '{name}' contains the separator character.");

                if (_valueLookup.ContainsKey(name))
                    throw new ArgumentException($"Duplicate enumeration member name '{name}'.");

                _valueLookup.Add(name, value);
                _nameLookup[value] = name;
            }

            static char GetSeparatorChar(string separator)
            {
                if (separator.Length == 1) {
                    return separator[0];
                }
                else if (separator.Length == 2) {
                    if (char.IsWhiteSpace(separator[0]) && !char.IsWhiteSpace(separator[1]))
                        return separator[1];

                    if (!char.IsWhiteSpace(separator[0]) && char.IsWhiteSpace(separator[1]))
                        return separator[0];
                }
                else if (separator.Length == 3) {
                    if (char.IsWhiteSpace(separator[0]) && !char.IsWhiteSpace(separator[1]) && char.IsWhiteSpace(separator[2]))
                        return separator[1];
                }

                throw new ArgumentException(
                    "Separator must contain exactly 1 separator character with an optional leading and trailing space.", nameof(separator));
            }
        }

        /// <summary>
        /// Gets a string representation of the specified enumeration value.
        /// </summary>
        /// <param name="value">The enumeration value.</param>
        /// <param name="allMatchingFlags">True if all matching flags should be included even if they are redundant, or false to only return a minimal set of
        /// flags. Ignored for non-flags enumerations.</param>
        public string ToString(T value, bool allMatchingFlags = false)
        {
            if (!Enum<T>.IsFlagsEnum)
                return GetNameOrNumericString(value);

            return string.Join(_toStringSeparator, value.SplitFlags(allMatchingFlags).Select(GetNameOrNumericString));

            // Gets the name of the specified value if it is defined, otherwise gets a string representation of its numeric value:

            string GetNameOrNumericString(T value)
            {
                if (_nameLookup.TryGetValue(value, out string str))
                    return str;

                return Convert.ChangeType(value, _underlyingType, CultureInfo.InvariantCulture).ToString()!;
            }
        }

        /// <summary>
        /// Gets the enumeration value parsed from the specified string.
        /// </summary>
        /// <param name="s">The string to be parsed.</param>
        public T Parse(string s)
        {
            if (TryParse(s, out T value))
                return value;

            throw new FormatException("Input string was not in a correct format.");
        }

        /// <summary>
        /// Parses the specified string into an enumeration value.
        /// </summary>
        /// <param name="s">The string to be parsed.</param>
        /// <param name="value">The resulting value.</param>
        /// <returns>True if parsing was successful, otherwise false.</returns>
        public bool TryParse(string s, out T value)
        {
            s = s.Trim();

            if (!Enum<T>.IsFlagsEnum)
                return TryGetNamedOrNumericValue(s, out value);

            if (s.Length == 0) {
                value = default;
                return true;
            }

            var splitOptions = char.IsWhiteSpace(_parseSeparatorArray[0]) ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
            string[] parts = s.Split(_parseSeparatorArray, splitOptions);

            value = default;

            for (int i = 0; i < parts.Length; i++) {
                string part = parts[i].Trim();

                if (part.Length > 0) {
                    if (!TryGetNamedOrNumericValue(part, out T partValue))
                        return false;

                    value = value.SetFlags(partValue);
                }
                else if (splitOptions == StringSplitOptions.None) {
                    return false;
                }
            }

            return true;

            bool TryGetNamedOrNumericValue(string s, out T value)
            {
                if (_valueLookup.TryGetValue(s, out value))
                    return true;

                try {
                    value = (T)Convert.ChangeType(s, _underlyingType, CultureInfo.InvariantCulture)!;
                    return true;
                }
                catch (FormatException) {
                    value = default;
                    return false;
                }
            }
        }
    }
}
