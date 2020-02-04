using System.Numerics;

namespace Veldrid.PBR
{
    public struct CommonMaterialParameters
    {
        public static readonly CommonMaterialParameters Default = new CommonMaterialParameters
        {
            BaseColorFactor = Vector4.One,
            MetallicFactor = 0,
            RoughnessFactor = 1
        };

        public Vector4 BaseColorFactor;
        public float MetallicFactor;
        public float RoughnessFactor;
    }
}