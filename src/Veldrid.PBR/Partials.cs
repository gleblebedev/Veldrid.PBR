using System.Numerics;

namespace Veldrid.PBR.Uniforms
{
    public partial struct MapUV
    {
        public static readonly MapUV Default = new MapUV {Set = 0, X = Vector3.UnitX, Y = Vector3.UnitY};
    }

    public partial struct UnlitMaterialArguments
    {
        public static readonly UnlitMaterialArguments Default = new UnlitMaterialArguments()
        {
            BaseColorFactor = Vector4.One,
            BaseColorMapUV = MapUV.Default,
            AlphaCutoff = 0.5f
        };
    }

    public partial struct MetallicRoughnessMaterialArguments
    {
        public static readonly MetallicRoughnessMaterialArguments Default = new MetallicRoughnessMaterialArguments()
        {
            BaseColorFactor = Vector4.One,
            BaseColorMapUV = MapUV.Default,
            AlphaCutoff = 0.5f,
            MetallicFactor = 0.0f,
            RoughnessFactor = 1.0f,
            EmissiveUV = MapUV.Default,
            MetallicRoughnessUV = MapUV.Default,
            NormalUV = MapUV.Default,
            OcclusionUV = MapUV.Default
        };
    }
    public partial struct SpecularGlossinessMaterialArguments
    {
        public static readonly SpecularGlossinessMaterialArguments Default = new SpecularGlossinessMaterialArguments()
        {
            DiffuseFactor = Vector4.One,
            DiffuseMapUV = MapUV.Default,
            AlphaCutoff = 0.5f,
            SpecularFactor = Vector3.One,
            GlossinessFactor = 1.0f,
            EmissiveUV = MapUV.Default,
            SpecularGlossinessUV = MapUV.Default,
            NormalUV = MapUV.Default,
            OcclusionUV = MapUV.Default
        };
    }
}