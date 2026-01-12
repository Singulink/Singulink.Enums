#if !NET
namespace Microsoft.CodeAnalysis;

#pragma warning disable CA1018 // Mark attributes with AttributeUsageAttribute
#pragma warning disable RCS1203 // Use AttributeUsageAttribute
internal sealed class EmbeddedAttribute : Attribute;
#endif
