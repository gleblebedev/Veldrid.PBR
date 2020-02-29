using System;

namespace Veldrid.PBR.BinaryData
{
    public struct IdRef
    {
        private readonly uint _index;

        public IdRef(uint index)
        {
            _index = index+1;
        }
        public IdRef(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index)+" should be positive number.");
            _index = (uint)index + 1;
        }
        public bool HasValue => _index != 0;
        public uint Value 
        { 
            get
            {
                if (_index == 0)
                    throw new IndexOutOfRangeException("No value available");
                return _index - 1;
            }
        }
    }

    public struct IdRef<T>
    {
        private readonly uint _index;

        public IdRef(uint index)
        {
            _index = index + 1;
        }
        public IdRef(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index) + " should be positive number.");
            _index = (uint)index + 1;
        }
        public bool HasValue => _index != 0;
        public uint Value
        {
            get
            {
                if (_index == 0)
                    throw new IndexOutOfRangeException("No value available");
                return _index - 1;
            }
        }

        public T Resolve(Func<uint, T> accessor)
        {
            if (_index == 0)
                return default(T);
            return accessor(_index - 1);
        }
    }
}