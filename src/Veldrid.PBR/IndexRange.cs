using System.Collections.Generic;

namespace Veldrid.PBR
{
    public struct IndexRange
    {
        public int StartIndex;
        public int Count;

        public IEnumerable<T> Enumerate<T>(IReadOnlyList<T> values)
        {
            var endIndex = StartIndex+Count;
            for (var index = StartIndex; index < endIndex; ++index)
            {
                yield return values[(int)index];
            }
        }
        public IEnumerable<T> Enumerate<T>(IList<T> values)
        {
            var endIndex = StartIndex + Count;
            for (var index = StartIndex; index < endIndex; ++index)
            {
                yield return values[(int)index];
            }
        }
    }
}