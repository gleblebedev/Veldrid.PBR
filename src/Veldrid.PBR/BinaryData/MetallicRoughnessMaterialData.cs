using Veldrid.PBR.Uniforms;

namespace Veldrid.PBR.BinaryData
{
    public struct MetallicRoughnessMaterialData
    {
        public AlphaMode AlphaMode;
        public FaceCullMode FaceCullMode;
        public MapAndSampler BaseColor;
        public MetallicRoughnessMaterialArguments UniformArguments;
        public MapAndSampler MetallicRoughness;
        public MapAndSampler Normal;
        public MapAndSampler Emissive;
        public MapAndSampler Occlusion;

        public static readonly MetallicRoughnessMaterialData Default = new MetallicRoughnessMaterialData()
        {
            AlphaMode = AlphaMode.Opaque,
            FaceCullMode = FaceCullMode.Back,
            UniformArguments = MetallicRoughnessMaterialArguments.Default
        };
    }
}