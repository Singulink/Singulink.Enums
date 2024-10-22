namespace Singulink.Enums;

/// <summary>
/// Represents an enumeration validator attribute that contains custom logic to execute when <see cref="EnumExtensions.IsValid{T}(T)"/> is called.
/// </summary>
public interface IEnumValidatorAttribute<T> where T : unmanaged, Enum
{
    /// <summary>
    /// Gets a value indicating whether the value is valid.
    /// </summary>
    bool IsValid(T value);
}
