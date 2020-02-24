using System;

namespace Veldrid.PBR.BinaryData
{
    public struct IdRef<T>
    {
        private readonly uint _index;

        public IdRef(uint index)
        {
            _index = index+1;
        }

        public T Resolve(Func<uint, T> accessor)
        {
            if (_index == 0)
                return default(T);
            return accessor(_index - 1);
        }
    }
}