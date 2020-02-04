using System.Numerics;

namespace Veldrid.PBR
{
    public struct MapParameters
    {
        public static readonly MapParameters Default = new MapParameters
        {
            AddressModeU = SamplerAddressMode.Wrap,
            AddressModeV = SamplerAddressMode.Wrap,
            AddressModeW = SamplerAddressMode.Wrap,
            Color = Vector4.Zero,
            MapIndex = -1,
            Scale = 1,
            UVSet = 0,
            UVTransform = Matrix4x4.Identity
        };

        public SamplerAddressMode AddressModeU;
        public SamplerAddressMode AddressModeV;
        public SamplerAddressMode AddressModeW;
        public Vector4 Color;
        public int MapIndex;
        public float Scale;
        public int UVSet;
        public Matrix4x4 UVTransform;
    }
}