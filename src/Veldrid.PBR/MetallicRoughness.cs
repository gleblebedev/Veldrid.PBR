namespace Veldrid.PBR
{
    public struct MetallicRoughness
    {
        public CommonMaterialParameters Common;
        public MapParameters BaseColor;

        public MapParameters MetallicRoughnessMap;
    }
}