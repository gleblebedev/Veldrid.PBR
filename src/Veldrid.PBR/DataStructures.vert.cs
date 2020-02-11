using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Veldrid.PBR.DataStructures
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct UVTransform
    {
        [FieldOffset(0)]
        public Vector3 X;

        [FieldOffset(16)]
        public Vector3 Y;

        [FieldOffset(28)]
        public uint Set;

    }
    [StructLayout(LayoutKind.Explicit)]
    public partial struct AllTypes
    {
        [FieldOffset(0)]
        public UVTransform S;

    }
    public static unsafe class ExtensionMethods
    {
    public static ref UVTransform AsUVTransform(this byte[] buffer, int offset = 0) { fixed (byte* ptr = buffer) { return ref *(UVTransform*)(ptr + offset); } }
    public static ref AllTypes AsAllTypes(this byte[] buffer, int offset = 0) { fixed (byte* ptr = buffer) { return ref *(AllTypes*)(ptr + offset); } }
    }
}
