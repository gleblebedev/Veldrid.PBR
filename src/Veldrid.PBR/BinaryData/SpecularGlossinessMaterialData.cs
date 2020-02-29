using Veldrid.PBR.Uniforms;

namespace Veldrid.PBR.BinaryData
{
    public struct SpecularGlossinessMaterialData
    {
        public AlphaMode AlphaMode;
        public FaceCullMode FaceCullMode;
        public MapAndSampler Diffuse;
        public SpecularGlossinessMaterialArguments UniformArguments;
        public MapAndSampler SpecularGlossiness;
        public MapAndSampler Normal;
        public MapAndSampler Emissive;
        public MapAndSampler Occlusion;

        public static readonly SpecularGlossinessMaterialData Default = new SpecularGlossinessMaterialData()
        {
            AlphaMode = AlphaMode.Opaque,
            FaceCullMode = FaceCullMode.Back,
            UniformArguments = SpecularGlossinessMaterialArguments.Default
        };
    }
}