using Veldrid.PBR.DataStructures;

namespace Veldrid.PBR.BinaryData
{
    public struct UnlitMaterialData
    {
        public MaterialDataBase Base;
        public MapUV BaseColorMapUV;
        public int BaseColorMap;
        public int BaseColorSampler;
    }
}