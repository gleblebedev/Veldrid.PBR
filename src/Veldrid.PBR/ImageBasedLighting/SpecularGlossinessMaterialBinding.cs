using Veldrid.PBR.Uniforms;

namespace Veldrid.PBR.ImageBasedLighting
{
    public class SpecularGlossinessMaterialBinding : MaterialBindingBase<SpecularGlossinessMaterialArguments>, IMaterial
    {
        private readonly SpecularGlossiness _material;

        public SpecularGlossinessMaterialBinding(SpecularGlossiness material,
            SimpleUniformPool<SpecularGlossinessMaterialArguments> uniformPool, GraphicsDevice graphicsDevice,
            ResourceCache resourceCache):base(uniformPool)
        {
            _material = material;
            Update();
        }

        public override void Update()
        {
            var args = new SpecularGlossinessMaterialArguments
            {
                DiffuseFactor = _material.BaseColorFactor,
                AlphaCutoff = _material.AlphaCutoff,
                DiffuseMapUV = _material.Diffuse.UV,
                NormalUV = _material.Normal.UV,
                EmissiveUV = _material.Emissive.UV,
                SpecularGlossinessUV = _material.SpecularGlossinessMap.UV,
                OcclusionUV = _material.Occlusion.UV,
                SpecularFactor = _material.SpecularFactor,
                GlossinessFactor = _material.GlossinessFactor
            };
            UpdateBuffer(ref args);
        }
    }
}