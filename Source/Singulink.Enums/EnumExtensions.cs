using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Singulink.Enums;

/// <summary>
/// Provides generic methods for bitwise operations and validation on enumerations.
/// </summary>
public static partial class EnumExtensions
{
    /// <summary>
    /// Gets a string representation of the specified enumeration value.
    /// </summary>
    /// <param name="value">The enumeration value.</param>
    public static string AsString<T>(this T value) where T : unmanaged, Enum
    {
        return EnumConverter<T>.Default.AsString(value);
    }

    /// <summary>
    /// Gets a string representation of the specified enumeration value using the specified options to customize how flags are split.
    /// </summary>
    /// <param name="value">The enumeration value.</param>
    /// <param name="flagsOptions">Options to customize the behavior of the conversion if the value is a flags enumeration. Ignored if the value is not a flags
    /// enumeration.</param>
    public static string AsString<T>(this T value, SplitFlagsOptions flagsOptions) where T : unmanaged, Enum
    {
        return EnumConverter<T>.Default.AsString(value, flagsOptions);
    }

    /// <summary>
    /// Determines whether the value's flags are all defined and they are a valid combination of flags.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreFlagsDefined<[DynamicallyAccessedMembers(PublicFields)] T>(this T value) where T : unmanaged, Enum
    {
        Enum<T>.EnsureIsFlagsEnum();

        if (EnumFlagsInfo<T>.AreAllFlagsDefinedBySingleBits)
            return EnumFlagsInfo<T>.AllSingleBitFlags.HasAllFlags(value);

        if (EnumFlagsInfo<T>.AllSingleBitFlags.HasAllFlags(value))
            return true;

        if (EnumFlagsInfo<T>.HasSingleMultiBitValue)
        {
            return value.HasAllFlags(EnumFlagsInfo<T>.SingleMultiBitValue) &&
                  EnumFlagsInfo<T>.AllSingleBitFlags.HasAllFlags(value.ClearFlags(EnumFlagsInfo<T>.SingleMultiBitValue));
        }

        foreach (var multiBitValue in EnumFlagsInfo<T>.MultiBitValuesDescending)
        {
            if (value.HasAllFlags(multiBitValue))
            {
                value = value.ClearFlags(multiBitValue);

                if (EnumFlagsInfo<T>.AllSingleBitFlags.HasAllFlags(value))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the value is defined.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefined<[DynamicallyAccessedMembers(PublicFields)] T>(this T value) where T : unmanaged, Enum
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
            return Enum<T>.DefaultIndex >= 0;

        if (Enum<T>.IsFlagsEnum)
        {
            if (value.HasSingleBitSet())
                return EnumFlagsInfo<T>.AllSingleBitFlags.HasAllFlags(value);

            if (EnumFlagsInfo<T>.HasSingleMultiBitValue)
                return EqualityComparer<T>.Default.Equals(value, EnumFlagsInfo<T>.SingleMultiBitValue);

            for (int i = 0; i < EnumFlagsInfo<T>.MultiBitValuesDescending.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(value, EnumFlagsInfo<T>.MultiBitValuesDescending[i]))
                    return true;
            }

            return false;
        }

        if (EnumRangeInfo<T>.IsContinuous)
        {
            return Comparer<T>.Default.Compare(value, EnumRangeInfo<T>.DefinedMin) >= 0 &&
                   Comparer<T>.Default.Compare(value, EnumRangeInfo<T>.DefinedMax) <= 0;
        }

        return Enum<T>.GetFirstValueIndex(value) >= 0;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if the value is not defined.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="paramName">The name of the parameter for the exception.</param>
    public static void ThrowIfNotDefined<[DynamicallyAccessedMembers(PublicFields)] TEnum>(this TEnum value, string paramName)
        where TEnum : unmanaged, Enum
    {
        if (!value.IsDefined())
            Throw(paramName);

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void Throw(string paramName) => throw new ArgumentOutOfRangeException(paramName, $"Undefined {typeof(TEnum).Name} value.");
    }

    /// <summary>
    /// Uses the custom <see cref="IEnumValidatorAttribute{T}"/> if available to check if the value is valid, otherwise checks whether the value is defined
    /// for regular enumerations, or checks whether the value's flags are all defined for flags enumerations.
    /// </summary>
    /// <remarks>
    /// <para>This method is equivalent to calling <see cref="IsDefined{T}(T)"/> on regular enumerations or <see cref="AreFlagsDefined{T}(T)"/> on flags
    /// enumerations that don't have custom validator attributes.</para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValid<[DynamicallyAccessedMembers(PublicFields)] T>(this T value) where T : unmanaged, Enum
    {
        if (EnumValidation<T>.ValidatorAttribute is not null and var attribute)
            return attribute.IsValid(value);

        return Enum<T>.IsFlagsEnum ? value.AreFlagsDefined() : value.IsDefined();
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if the value is not valid.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="paramName">The name of the parameter for the exception.</param>
    public static void ThrowIfNotValid<[DynamicallyAccessedMembers(PublicFields)] TEnum>(this TEnum value, string paramName)
        where TEnum : unmanaged, Enum
    {
        if (!value.IsValid())
            Throw(paramName);

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void Throw(string paramName) => throw new ArgumentOutOfRangeException(paramName, $"Invalid {typeof(TEnum).Name} value.");
    }

    /// <summary>
    /// Gets the first enumeration name with the given value.
    /// </summary>
    /// <param name="value">The enumeration value.</param>
    /// <exception cref="MissingMemberException">An enumeration with the specified value was not found.</exception>
    public static string GetName<[DynamicallyAccessedMembers(PublicFields)] T>(this T value) where T : unmanaged, Enum
    {
        if (!TryGetName(value, out string name))
        {
            [DoesNotReturn]
            static void Throw(T value) => throw new MissingMemberException($"An enumeration with the value '{value}' was not found.");
            Throw(value);
        }

        return name;
    }

    /// <summary>
    /// Gets the first enumeration name with the given value.
    /// </summary>
    /// <param name="value">The enumeration value.</param>
    /// <param name="name">Contains the matched name when the method returns if the name was found, otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the name was found, otherwise <see langword="false"/>.</returns>
    public static bool TryGetName<[DynamicallyAccessedMembers(PublicFields)] T>(this T value, [NotNullWhen(true)] out string? name)
        where T : unmanaged, Enum
    {
        int index = Enum<T>.GetFirstValueIndex(value);

        if (index < 0)
        {
            name = null;
            return false;
        }

        name = Enum<T>.Names[index];
        return true;
    }

    internal static unsafe bool HasSingleBitSet<T>(this T value) where T : unmanaged, Enum
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
            return false;

        if (sizeof(T) is 1)
        {
            byte v = UnsafeMethods.BitCast<T, byte>(value);
            return (v & (v - 1)) == 0;
        }
        else if (sizeof(T) is 2)
        {
            ushort v = UnsafeMethods.BitCast<T, ushort>(value);
            return (v & (v - 1)) == 0;
        }
        else if (sizeof(T) is 4)
        {
            uint v = UnsafeMethods.BitCast<T, uint>(value);
            return (v & (v - 1)) == 0;
        }
        else if (sizeof(T) is 8)
        {
            ulong v = UnsafeMethods.BitCast<T, ulong>(value);
            return (v & (v - 1)) == 0;
        }

        throw new NotSupportedException();
    }
}
