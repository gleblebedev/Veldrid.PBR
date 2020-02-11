using System.Collections.Generic;

namespace Veldrid.PBR
{
    internal class VertexAttributeComparer : IComparer<AbstractVertexAttribute>
    {
        static IComparer<int> _intComparer = Comparer<int>.Default;
        static IComparer<string> _strComparer = Comparer<string>.Default;
        public int Compare(AbstractVertexAttribute x, AbstractVertexAttribute y)
        {
            var res = _intComparer.Compare(x.Priority, y.Priority);
            if (res == 0)
                res = _strComparer.Compare(x.Key, y.Key);
            return res;
        }
    }
}