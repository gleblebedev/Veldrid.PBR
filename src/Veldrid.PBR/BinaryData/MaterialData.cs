using System.Numerics;

namespace Veldrid.PBR.BinaryData
{
    public struct MaterialDataBase
    {
        public static readonly MaterialDataBase Default = new MaterialDataBase
        {
            BaseColorFactor = Vector4.One,
            AlphaMode = AlphaMode.Opaque,
            AlphaCutoff = 1.0f
        };

        public Vector4 BaseColorFactor;
        public AlphaMode AlphaMode;
        public float AlphaCutoff;
    }
}