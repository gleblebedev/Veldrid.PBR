using Veldrid.PBR.Uniforms;
using Veldrid.PBR.Unlit;

namespace Veldrid.PBR.ImageBasedLighting
{
    public class UnlitMaterialBinding : MaterialBindingBase<UnlitMaterialArguments>, IMaterial
    {
        private readonly UnlitMaterial _material;

        public UnlitMaterialBinding(UnlitMaterial material,
            SimpleUniformPool<UnlitMaterialArguments> uniformPool,
            GraphicsDevice graphicsDevice,
            ResourceCache resourceCache) : base(uniformPool)
        {
            _material = material;
            ShaderFlags = UnlitShaderFlags.None;
            if (material.BaseColorMap.Map != null)
            {
                ShaderFlags |= UnlitShaderFlags.HasBaseColorMap;
                var resourceLayoutDescription = new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("UnlitMaterialArguments", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.DynamicBinding),
                    new ResourceLayoutElementDescription("BaseColorTexture", ResourceKind.TextureReadOnly,
                        ShaderStages.Fragment, ResourceLayoutElementOptions.None),
                    new ResourceLayoutElementDescription("BaseColorSampler", ResourceKind.Sampler,
                        ShaderStages.Fragment, ResourceLayoutElementOptions.None)
                );
                ResourceLayout = resourceCache.GetResourceLayout(resourceLayoutDescription);
                var resourceSetDescription = new ResourceSetDescription(
                    ResourceLayout,
                    uniformPool.BindableResource,
                    material.BaseColorMap.Map,
                    material.BaseColorMap.Sampler ?? graphicsDevice.Aniso4xSampler);
                ResourceSet = new ResourceSetAndOffsets(resourceCache.GetResourceSet(resourceSetDescription), _offset);
            }
            else
            {
                var resourceLayoutDescription = new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("UnlitMaterialArguments", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.DynamicBinding)
                );
                ResourceLayout = resourceCache.GetResourceLayout(resourceLayoutDescription);
                var resourceSetDescription = new ResourceSetDescription(
                    ResourceLayout,
                    uniformPool.BindableResource);
                ResourceSet = new ResourceSetAndOffsets(graphicsDevice.ResourceFactory.CreateResourceSet(resourceSetDescription), _offset);
            }

            Update();
        }

        public ResourceLayout ResourceLayout { get; }

        public ResourceSetAndOffsets ResourceSet { get; }

        public UnlitShaderFlags ShaderFlags { get; }

        public FaceCullMode CullMode => _material.FaceCullMode;

        public override void Update()
        {
            var args = new UnlitMaterialArguments
            {
                BaseColorFactor = _material.BaseColorFactor,
                AlphaCutoff = _material.AlphaCutoff,
                BaseColorMapUV = _material.BaseColorMap.UV
            };
            UpdateBuffer(ref args);
        }
    }
}