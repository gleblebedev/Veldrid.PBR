using Veldrid.PBR.DataStructures;

namespace Veldrid.PBR.BinaryData
{
    public struct UnlitMaterialData
    {
        public static readonly UnlitMaterialData Default = new UnlitMaterialData()
        {
            BaseColorMap = -1,
            BaseColorSampler = -1,
            BaseColorMapUV = MapUV.Default,
            Base = MaterialDataBase.Default
        };
        public MaterialDataBase Base;
        public MapUV BaseColorMapUV;
        public int BaseColorMap;
        public int BaseColorSampler;
    }
}