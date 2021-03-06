﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Veldrid.PBR.BinaryData
{
    public struct Chunk<T> where T : struct
    {
        public int Offset;
        public int Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetAt<TMemory>(in Memory<TMemory> data, int index) where TMemory : struct
        {
            return ref MemoryMarshal.Cast<TMemory, T>(data.Span.Slice(Offset))[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetAt<TMemory>(in Memory<TMemory> data, uint index) where TMemory : struct
        {
            return ref MemoryMarshal.Cast<TMemory, T>(data.Span.Slice(Offset))[(int)index];
        }
    }
}