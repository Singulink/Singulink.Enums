using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Singulink.Enums
{
    /// <summary>
    /// Provides generic methods for bitwise operations and validation on enumerations.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Uses the custom <see cref="IEnumValidatorAttribute{T}"/> if available to check if the value is valid, otherwise checks whether the value is defined
        /// for regular enumerations, or checks whether the value's flags are all defined for flags enumerations.
        /// </summary>
        /// <remarks>
        /// <para>This method is equivalent to calling <see cref="IsDefined{T}(T)"/> on regular enumerations or <see cref="AreFlagsDefined{T}(T)"/> on flags
        /// enumerations that don't have custom validator attributes.</para>
        /// </remarks>
        public static bool IsValid<T>(this T value) where T : unmanaged, Enum
        {
            if (EnumValidation<T>.ValidatorAttribute is { } attribute)
                return attribute.IsValid(value);

            return Enum<T>.IsFlagsEnum ? value.AreFlagsDefined() : value.IsDefined();
        }

        /// <summary>
        /// Determines whether the value is defined.
        /// </summary>
        public static bool IsDefined<T>(this T value) where T : unmanaged, Enum
        {
            if (EnumRangeInfo<T>.IsContinuous) {
                var comparer = Comparer<T>.Default;
                int result = comparer.Compare(value, EnumRangeInfo<T>.DefinedMin);

                return result == 0 || (result > 0 && comparer.Compare(value, EnumRangeInfo<T>.DefinedMax) <= 0);
            }

            return Enum<T>.Values.BinarySearch(value, Enum<T>.ValueComparer) >= 0;
        }

        /// <inheritdoc cref="Enum{T}.TryGetName(T, out string?)"/>
        public static bool TryGetName<T>(this T value, [NotNullWhen(true)] out string? name) where T : unmanaged, Enum => Enum<T>.TryGetName(value, out name);

        /// <inheritdoc cref="Enum{T}.GetName(T)"/>
        public static string GetName<T>(this T value) where T : unmanaged, Enum => Enum<T>.GetName(value);

        /// <summary>
        /// Determines whether the value's flags are all defined.
        /// </summary>
        public static bool AreFlagsDefined<T>(this T value) where T : unmanaged, Enum => EnumFlagsInfo<T>.AllDefinedFlags.HasAllFlags(value);

        /// <summary>
        /// Determines whether the value has all the given flags.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool HasAllFlags<T>(this T value, T flags) where T : unmanaged, Enum
        {
            if (sizeof(T) == 1)
                return (Unsafe.As<T, byte>(ref value) | Unsafe.As<T, byte>(ref flags)) == Unsafe.As<T, byte>(ref value);
            else if (sizeof(T) == 2)
                return (Unsafe.As<T, short>(ref value) | Unsafe.As<T, short>(ref flags)) == Unsafe.As<T, short>(ref value);
            else if (sizeof(T) == 4)
                return (Unsafe.As<T, int>(ref value) | Unsafe.As<T, int>(ref flags)) == Unsafe.As<T, int>(ref value);
            else if (sizeof(T) == 8)
                return (Unsafe.As<T, long>(ref value) | Unsafe.As<T, long>(ref flags)) == Unsafe.As<T, long>(ref value);

            throw new NotSupportedException();
        }

        /// <inheritdoc cref="HasAllFlags{T}(T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAllFlags<T>(this T value, T flags1, T flags2) where T : unmanaged, Enum
        {
            return value.HasAllFlags(flags1.SetFlags(flags2));
        }

        /// <inheritdoc cref="HasAllFlags{T}(T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAllFlags<T>(this T value, T flags1, T flags2, T flags3) where T : unmanaged, Enum
        {
            return value.HasAllFlags(flags1.SetFlags(flags2).SetFlags(flags3));
        }

        /// <inheritdoc cref="HasAllFlags{T}(T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAllFlags<T>(this T value, params T[] flags) where T : unmanaged, Enum
        {
            return value.HasAllFlags(default(T).SetFlags(flags));
        }

        /// <inheritdoc cref="HasAllFlags{T}(T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAllFlags<T>(this T value, IEnumerable<T> flags) where T : unmanaged, Enum
        {
            return value.HasAllFlags(default(T).SetFlags(flags));
        }

        /// <summary>
        /// Determines whether the value has any of the given flags.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool HasAnyFlag<T>(this T value, T flags) where T : unmanaged, Enum
        {
            if (sizeof(T) == 1)
                return (Unsafe.As<T, byte>(ref value) & Unsafe.As<T, byte>(ref flags)) != 0;
            else if (sizeof(T) == 2)
                return (Unsafe.As<T, short>(ref value) & Unsafe.As<T, short>(ref flags)) != 0;
            else if (sizeof(T) == 4)
                return (Unsafe.As<T, int>(ref value) & Unsafe.As<T, int>(ref flags)) != 0;
            else if (sizeof(T) == 8)
                return (Unsafe.As<T, long>(ref value) & Unsafe.As<T, long>(ref flags)) != 0;

            throw new NotSupportedException();
        }

        /// <inheritdoc cref="HasAnyFlag{T}(T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyFlag<T>(this T value, T flags1, T flags2) where T : unmanaged, Enum
        {
            return value.HasAnyFlag(flags1.SetFlags(flags2));
        }

        /// <inheritdoc cref="HasAnyFlag{T}(T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyFlag<T>(this T value, T flags1, T flags2, T flags3) where T : unmanaged, Enum
        {
            return value.HasAnyFlag(flags1.SetFlags(flags2).SetFlags(flags3));
        }

        /// <inheritdoc cref="HasAnyFlag{T}(T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyFlag<T>(this T value, params T[] flags) where T : unmanaged, Enum
        {
            return value.HasAnyFlag(default(T).SetFlags(flags));
        }

        /// <inheritdoc cref="HasAnyFlag{T}(T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyFlag<T>(this T value, IEnumerable<T> flags) where T : unmanaged, Enum
        {
            return value.HasAnyFlag(default(T).SetFlags(flags));
        }

        /// <summary>
        /// Sets the specified flags on the value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T SetFlags<T>(this T value, T flags) where T : unmanaged, Enum
        {
            if (sizeof(T) == 1)
                return UnsafeValue.As<byte, T>((byte)(Unsafe.As<T, byte>(ref value) | Unsafe.As<T, byte>(ref flags)));
            else if (sizeof(T) == 2)
                return UnsafeValue.As<short, T>((short)(Unsafe.As<T, short>(ref value) | Unsafe.As<T, short>(ref flags)));
            else if (sizeof(T) == 4)
                return UnsafeValue.As<int, T>(Unsafe.As<T, int>(ref value) | Unsafe.As<T, int>(ref flags));
            else if (sizeof(T) == 8)
                return UnsafeValue.As<long, T>(Unsafe.As<T, long>(ref value) | Unsafe.As<T, long>(ref flags));

            throw new NotSupportedException();
        }

        /// <inheritdoc cref="SetFlags{T}(T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetFlags<T>(this T value, T flags1, T flags2) where T : unmanaged, Enum
        {
            return value.SetFlags(flags1).SetFlags(flags2);
        }

        /// <inheritdoc cref="SetFlags{T}(T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetFlags<T>(this T value, T flags1, T flags2, T flags3) where T : unmanaged, Enum
        {
            return value.SetFlags(flags1).SetFlags(flags2).SetFlags(flags3);
        }

        /// <inheritdoc cref="SetFlags{T}(T, T)"/>
        public static T SetFlags<T>(this T value, params T[] flags) where T : unmanaged, Enum
        {
            foreach (var flag in flags)
                value = value.SetFlags(flag);

            return value;
        }

        /// <inheritdoc cref="SetFlags{T}(T, T)"/>
        public static T SetFlags<T>(this T value, IEnumerable<T> flags) where T : unmanaged, Enum
        {
            foreach (var flag in flags)
                value = value.SetFlags(flag);

            return value;
        }

        /// <summary>
        /// Clears the specified flags on the value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T ClearFlags<T>(this T value, T flags) where T : unmanaged, Enum
        {
            if (sizeof(T) == 1)
                return UnsafeValue.As<byte, T>((byte)(Unsafe.As<T, byte>(ref value) & ~Unsafe.As<T, byte>(ref flags)));
            else if (sizeof(T) == 2)
                return UnsafeValue.As<short, T>((short)(Unsafe.As<T, short>(ref value) & ~Unsafe.As<T, short>(ref flags)));
            else if (sizeof(T) == 4)
                return UnsafeValue.As<int, T>(Unsafe.As<T, int>(ref value) & ~Unsafe.As<T, int>(ref flags));
            else if (sizeof(T) == 8)
                return UnsafeValue.As<long, T>(Unsafe.As<T, long>(ref value) & ~Unsafe.As<T, long>(ref flags));

            throw new NotSupportedException();
        }

        /// <inheritdoc cref="ClearFlags{T}(T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ClearFlags<T>(this T value, T flags1, T flags2) where T : unmanaged, Enum
        {
            return value.ClearFlags(flags1).ClearFlags(flags2);
        }

        /// <inheritdoc cref="ClearFlags{T}(T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ClearFlags<T>(this T value, T flags1, T flags2, T flags3) where T : unmanaged, Enum
        {
            return value.ClearFlags(flags1).ClearFlags(flags2).ClearFlags(flags3);
        }

        /// <inheritdoc cref="ClearFlags{T}(T, T)"/>
        public static T ClearFlags<T>(this T value, params T[] flags) where T : unmanaged, Enum
        {
            foreach (var flag in flags)
                value = value.ClearFlags(flag);

            return value;
        }

        /// <inheritdoc cref="ClearFlags{T}(T, T)"/>
        public static T ClearFlags<T>(this T value, IEnumerable<T> flags) where T : unmanaged, Enum
        {
            foreach (var flag in flags)
                value = value.ClearFlags(flag);

            return value;
        }

        /// <summary>
        /// Splits the value into the defined flags that make up the value (plus any remainder).
        /// </summary>
        /// <param name="value">The value to split.</param>
        /// <param name="allMatchingFlags">True if all matching flags should be included even if they are redundant, or false to only return a minimal set of flags.</param>
        public static IEnumerable<T> SplitFlags<T>(this T value, bool allMatchingFlags = false) where T : unmanaged, Enum
        {
            // This method depends on special enum flag sorting order of values to work properly.
            if (!Enum<T>.IsFlagsEnum)
                throw new InvalidOperationException($"Type '{typeof(T)}' is not a flags enumeration.");

            if (EqualityComparer<T>.Default.Equals(value, default)) {
                if (default(T).IsDefined())
                    return new[] { default(T) };

                return Array.Empty<T>();
            }

            var results = new List<T>();
            T remainder = value;

            for (int i = Enum<T>.Values.Length - 1; i >= 0; i--) {
                var definedValue = Enum<T>.Values[i];

                if (!EqualityComparer<T>.Default.Equals(definedValue, default) && value.HasAllFlags(definedValue)) {
                    results.Add(definedValue);

                    if (!allMatchingFlags)
                        value = value.ClearFlags(definedValue);

                    remainder = remainder.ClearFlags(definedValue);
                }
            }

            results.Reverse();

            if (!EqualityComparer<T>.Default.Equals(remainder, default))
                results.Add(remainder);

            return results;
        }

        private static class UnsafeValue
        {
            public static TTo As<TFrom, TTo>(TFrom source) => Unsafe.As<TFrom, TTo>(ref source);
        }
    }
}
