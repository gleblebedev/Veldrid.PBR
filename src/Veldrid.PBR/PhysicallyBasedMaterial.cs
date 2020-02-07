using System.Numerics;

namespace Veldrid.PBR
{
    public class PhysicallyBasedMaterial
    {
        public Vector4 BaseColorFactor { get; set; } = Vector4.One;
        public float MetallicFactor { get; set; } = 0.0f;
        public float RoughnessFactor { get; set; } = 1.0f;
        public MapParameters Normal { get; set; }
        public MapParameters Emissive { get; set; }
        public MapParameters Occlusion { get; set; }
        public bool DepthTestEnabled { get; set; } = true;
        public bool DepthWriteEnabled { get; set; } = true;
        public AlphaMode AlphaMode { get; set; } = AlphaMode.Opaque;
        public float AlphaCutoff { get; set; } = 1.0f;
        public SpecularGlossiness SpecularGlossiness { get; set; }
        public MetallicRoughness MetallicRoughness { get; set; }
        public bool Unlit { get; set; } = false;
    }
}