using System.Collections;
using System.Collections.Generic;

namespace Veldrid.PBR
{
    public struct IndexRange : IEnumerable<int>
    {
        public int StartIndex;
        public int Count;

        public IndexRange(int start, int count)
        {
            StartIndex = start;
            Count = count;
        }

        public IEnumerable<T> Enumerate<T>(IReadOnlyList<T> values)
        {
            var endIndex = StartIndex + Count;
            for (var index = StartIndex; index < endIndex; ++index) yield return values[index];
        }

        public IEnumerable<T> Enumerate<T>(IList<T> values)
        {
            var endIndex = StartIndex + Count;
            for (var index = StartIndex; index < endIndex; ++index) yield return values[index];
        }

        public IEnumerator<int> GetEnumerator()
        {
            var endIndex = StartIndex + Count;
            for (var index = StartIndex; index < endIndex; ++index) yield return index;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}