using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Veldrid.PBR.Uniforms
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
        public Vector4 BaseColorFactor;

        [FieldOffset(16)]
        public MapUV BaseColorMapUV;

        [FieldOffset(48)]
        public float AlphaCutoff;
    }
    [StructLayout(LayoutKind.Explicit)]
    public partial struct UnlitMaterialArgumentsUniform
    {
        [FieldOffset(0)]
        public UnlitMaterialArguments unlitArgs;
    }
    [StructLayout(LayoutKind.Explicit)]
    public partial struct NodeProperties
    {
        [FieldOffset(0)]
        public Matrix4x4 WorldTransform;
    }
    [StructLayout(LayoutKind.Explicit)]
    public partial struct ViewProjection
    {
        [FieldOffset(0)]
        public Matrix4x4 View;

        [FieldOffset(64)]
        public Matrix4x4 Projection;
    }
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MetallicRoughnessMaterialArguments
    {
        [FieldOffset(0)]
        public Vector4 BaseColorFactor;

        [FieldOffset(16)]
        public MapUV BaseColorMapUV;

        [FieldOffset(48)]
        public MapUV MetallicRoughnessUV;

        [FieldOffset(80)]
        public MapUV NormalUV;

        [FieldOffset(112)]
        public MapUV EmissiveUV;

        [FieldOffset(144)]
        public MapUV OcclusionUV;

        [FieldOffset(176)]
        public float MetallicFactor;

        [FieldOffset(180)]
        public float RoughnessFactor;

        [FieldOffset(184)]
        public float AlphaCutoff;
    }
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MetallicRoughnessMaterialArgumentsUniform
    {
        [FieldOffset(0)]
        public MetallicRoughnessMaterialArguments metallicRoughnessArgs;
    }
    [StructLayout(LayoutKind.Explicit)]
    public partial struct SpecularGlossinessMaterialArguments
    {
        [FieldOffset(0)]
        public Vector4 DiffuseFactor;

        [FieldOffset(16)]
        public MapUV DiffuseMapUV;

        [FieldOffset(48)]
        public MapUV SpecularGlossinessUV;

        [FieldOffset(80)]
        public MapUV NormalUV;

        [FieldOffset(112)]
        public MapUV EmissiveUV;

        [FieldOffset(144)]
        public MapUV OcclusionUV;

        [FieldOffset(176)]
        public Vector3 SpecularFactor;

        [FieldOffset(188)]
        public float GlossinessFactor;

        [FieldOffset(192)]
        public float AlphaCutoff;
    }
    [StructLayout(LayoutKind.Explicit)]
    public partial struct SpecularGlossinessMaterialArgumentsUniform
    {
        [FieldOffset(0)]
        public SpecularGlossinessMaterialArguments specularGlossinessArgs;
    }
}
