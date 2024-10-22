using System.Diagnostics.CodeAnalysis;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Singulink.Enums;

/// <summary>
/// Provides generic methods for bitwise operations and validation on enumerations.
/// </summary>
public static partial class EnumExtensions
{
    /// <summary>
    /// Determines whether the value's flags are all defined.
    /// </summary>
    public static bool AreFlagsDefined<T>(this T value) where T : unmanaged, Enum => EnumFlagsInfo<T>.AllDefinedFlags.HasAllFlags(value);

    /// <summary>
    /// Uses the custom <see cref="IEnumValidatorAttribute{T}"/> if available to check if the value is valid, otherwise checks whether the value is defined
    /// for regular enumerations, or checks whether the value's flags are all defined for flags enumerations.
    /// </summary>
    /// <remarks>
    /// <para>This method is equivalent to calling <see cref="IsDefined{T}(T)"/> on regular enumerations or <see cref="AreFlagsDefined{T}(T)"/> on flags
    /// enumerations that don't have custom validator attributes.</para>
    /// </remarks>
    public static bool IsValid<[DynamicallyAccessedMembers(PublicFields)] T>(this T value) where T : unmanaged, Enum
    {
        if (EnumValidation<T>.ValidatorAttribute is not null and var attribute)
            return attribute.IsValid(value);

        return Enum<T>.IsFlagsEnum ? value.AreFlagsDefined() : value.IsDefined();
    }

    /// <summary>
    /// Determines whether the value is defined.
    /// </summary>
    public static bool IsDefined<[DynamicallyAccessedMembers(PublicFields)] T>(this T value) where T : unmanaged, Enum
    {
        if (EnumRangeInfo<T>.IsContinuous) {
            int result = Comparer<T>.Default.Compare(value, EnumRangeInfo<T>.DefinedMin);

            return result is 0 || (result > 0 && Comparer<T>.Default.Compare(value, EnumRangeInfo<T>.DefinedMax) <= 0);
        }

        return Enum<T>.BinarySearchValues(value) >= 0;
    }

    /// <summary>
    /// Gets the first enumeration name with the given value.
    /// </summary>
    /// <param name="value">The enumeration value.</param>
    /// <exception cref=" MissingMemberException">An enumeration with the specified value was not found.</exception>
    public static string GetName<[DynamicallyAccessedMembers(PublicFields)] T>(this T value) where T : unmanaged, Enum
    {
        if (!TryGetName(value, out string name))
            throw new MissingMemberException($"An enumeration with the value '{value}' was not found.");

        return name;
    }

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
    /// Gets the first enumeration name with the given value.
    /// </summary>
    /// <param name="value">The enumeration value.</param>
    /// <param name="name">Contains the matched name when the method returns if the name was found, otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the name was found, otherwise <see langword="false"/>.</returns>
    public static bool TryGetName<[DynamicallyAccessedMembers(PublicFields)] T>(this T value, [NotNullWhen(true)] out string? name)
        where T : unmanaged, Enum
    {
        int index = Enum<T>.BinarySearchValues(value);

        if (index < 0)
        {
            name = null;
            return false;
        }

        name = Enum<T>.Names[index];
        return true;
    }
}
