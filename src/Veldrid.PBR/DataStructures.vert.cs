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
        public int Set;

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
    public partial struct NodeProperties
    {
        [FieldOffset(0)]
        public Matrix4x4 WorldTransform;
    }
}
