#if !NET
using Microsoft.CodeAnalysis;

namespace Singulink.Enums;

[Embedded]
internal static class StringExtensions
{
    extension(string s)
    {
        public void CopyTo(Span<char> destination) => s.AsSpan().CopyTo(destination);
    }
}
#endif
