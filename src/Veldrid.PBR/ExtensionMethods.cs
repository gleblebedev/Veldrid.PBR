using System;

namespace Veldrid.PBR
{
    internal static class ExtensionMethods
    {
        public static Span<T> Slice<T>(this in Span<T> span, IndexRange range)
        {
            return span.Slice(range.StartIndex, range.Count);
        }
    }
}