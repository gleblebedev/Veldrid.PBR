namespace Veldrid.PBR
{
    public class PhysicallyBasedMaterial: MaterialBase
    {
        public MapParameters Normal { get; set; }
        public MapParameters Emissive { get; set; }
        public MapParameters Occlusion { get; set; }
    }
}