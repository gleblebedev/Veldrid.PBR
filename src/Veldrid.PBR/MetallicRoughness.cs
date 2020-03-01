namespace Veldrid.PBR
{
    public class MetallicRoughness : PhysicallyBasedMaterial
    {
        public MapParameters BaseColor;

        public MapParameters MetallicRoughnessMap;

        public float MetallicFactor { get; set; } = 0.0f;

        public float RoughnessFactor { get; set; } = 1.0f;
    }
}