using Veldrid.PBR.Uniforms;

namespace Veldrid.PBR.BinaryData
{
    public struct UnlitMaterialData
    {
        public AlphaMode AlphaMode;
        public FaceCullMode FaceCullMode;
        public MapAndSampler BaseColor;
        public UnlitMaterialArguments UniformArguments;

        public static readonly UnlitMaterialData Default = new UnlitMaterialData()
        {
            AlphaMode = AlphaMode.Opaque,
            FaceCullMode = FaceCullMode.Back,
            UniformArguments = UnlitMaterialArguments.Default
        };
    }
}