using System;
using System.Runtime.CompilerServices;

namespace Veldrid.PBR
{
    internal struct BitMask
    {
        private readonly ulong[] _bits;

        public BitMask(uint numBits)
        {
            _bits = new ulong[(int) ((numBits + 63) / 64)];
        }

        public bool this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 0 != (_bits[index >> 64] & (1ul << ((int) index & 63)));
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value)
                    SetAt(index);
                else
                    ResetAt(index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint FindFirstAvailableBit()
        {
            for (var index = 0; index < _bits.Length; index++)
            {
                var mask = _bits[index];
                if (mask != ~0ul)
                {
                    ulong bitMask = 1;
                    for (var subIndex = 0; subIndex < 64; ++subIndex)
                    {
                        if (0ul == (mask & bitMask)) return (uint) index * 64 + (uint) subIndex;

                        bitMask <<= 1;
                    }
                }
            }

            throw new IndexOutOfRangeException("No more elements available.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAt(uint index)
        {
            var arrayIndex = index >> 6;
            _bits[arrayIndex] |= 1ul << ((int) index & 63);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetAt(uint index)
        {
            _bits[index >> 6] &= ~(1ul << ((int) index & 63));
        }
    }
}