using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Veldrid.PBR.DataStructures
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MapUV
    {
        [FieldOffset(0)]
        public Vector3 X;

        [FieldOffset(12)]
        public uint Set;

        [FieldOffset(16)]
        public Vector3 Y;

    }
    [StructLayout(LayoutKind.Explicit)]
    public partial struct UnlitMaterialArguments
    {
        [FieldOffset(0)]
        public MapUV BaseColorMapUV;

    }
    [StructLayout(LayoutKind.Explicit)]
    public partial struct AllTypes
    {
        [FieldOffset(0)]
        public UnlitMaterialArguments S;

    }
    public static unsafe class ExtensionMethods
    {
    public static ref MapUV AsMapUV(this byte[] buffer, int offset = 0) { fixed (byte* ptr = buffer) { return ref *(MapUV*)(ptr + offset); } }
    public static ref UnlitMaterialArguments AsUnlitMaterialArguments(this byte[] buffer, int offset = 0) { fixed (byte* ptr = buffer) { return ref *(UnlitMaterialArguments*)(ptr + offset); } }
    public static ref AllTypes AsAllTypes(this byte[] buffer, int offset = 0) { fixed (byte* ptr = buffer) { return ref *(AllTypes*)(ptr + offset); } }
    }
}
