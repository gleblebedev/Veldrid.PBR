using System.Numerics;

namespace Veldrid.PBR
{
    public class MaterialBase
    {
        public Vector4 BaseColorFactor { get; set; } = Vector4.One;
        public AlphaMode AlphaMode { get; set; } = AlphaMode.Opaque;
        public float AlphaCutoff { get; set; } = 1.0f;
    }
}