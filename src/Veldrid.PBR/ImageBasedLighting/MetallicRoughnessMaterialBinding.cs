using Veldrid.PBR.Uniforms;

namespace Veldrid.PBR.ImageBasedLighting
{
    public class MetallicRoughnessMaterialBinding : MaterialBindingBase<MetallicRoughnessMaterialArguments>, IMaterial
    {
        private readonly MetallicRoughness _material;

        public MetallicRoughnessMaterialBinding(MetallicRoughness material,
            SimpleUniformPool<MetallicRoughnessMaterialArguments> uniformPool, GraphicsDevice graphicsDevice,
            ResourceCache resourceCache) : base(uniformPool)
        {
            _material = material;
            Update();
        }

        public override void Update()
        {
            var args = new MetallicRoughnessMaterialArguments
            {
                BaseColorFactor = _material.BaseColorFactor,
                AlphaCutoff = _material.AlphaCutoff,
                BaseColorMapUV = _material.BaseColor.UV,
                NormalUV = _material.Normal.UV,
                EmissiveUV = _material.Emissive.UV,
                MetallicRoughnessUV = _material.MetallicRoughnessMap.UV,
                OcclusionUV = _material.Occlusion.UV,
                MetallicFactor = _material.MetallicFactor,
                RoughnessFactor = _material.RoughnessFactor
            };
            UpdateBuffer(ref args);
        }
    }
}