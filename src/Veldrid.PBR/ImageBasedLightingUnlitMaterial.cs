using System.Numerics;
using Veldrid.PBR.DataStructures;

namespace Veldrid.PBR
{
    public class ImageBasedLightingUnlitMaterial:IMaterial
    {
        private readonly UnlitMaterial _material;
        private readonly SimpleUniformPool<UnlitMaterialArguments> _uniformPool;
        private readonly GraphicsDevice _graphicsDevice;
        private uint _offset;

        public ImageBasedLightingUnlitMaterial(UnlitMaterial material, SimpleUniformPool<UnlitMaterialArguments> uniformPool, GraphicsDevice graphicsDevice)
        {
            _material = material;
            _uniformPool = uniformPool;
            _graphicsDevice = graphicsDevice;
            _offset = _uniformPool.Allocate();
            Update();
        }

        public void Update()
        {
            UnlitMaterialArguments args = new UnlitMaterialArguments()
            {
                BaseColorFactor = _material.BaseColorFactor,
                AlphaCutoff = _material.AlphaCutoff,
                BaseColorMapUV = _material.BaseColorMap.UV
            };
            _uniformPool.UpdateBuffer(_offset, ref args);
        }

        public uint UniformOffset => _offset;

        public void Dispose()
        {
            _uniformPool.Release(_offset);
        }
    }
}