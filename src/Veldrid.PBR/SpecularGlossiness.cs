using System.Numerics;

namespace Veldrid.PBR
{
    public class SpecularGlossiness : PhysicallyBasedMaterial
    {
        public MapParameters Diffuse;

        public MapParameters SpecularGlossinessMap;

        public Vector3 SpecularFactor { get; set; } = Vector3.One;

        public float GlossinessFactor { get; set; } = 1.0f;
    }
}