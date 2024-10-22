using System.Reflection;

namespace Singulink.Enums;

internal static class EnumValidation<T> where T : unmanaged, Enum
{
    internal static readonly IEnumValidatorAttribute<T>? ValidatorAttribute =
        typeof(T).GetCustomAttributes().OfType<IEnumValidatorAttribute<T>>().SingleOrDefault();
}
