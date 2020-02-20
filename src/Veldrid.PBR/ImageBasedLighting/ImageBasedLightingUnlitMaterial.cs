using Veldrid.PBR.DataStructures;
using Veldrid.PBR.Unlit;

namespace Veldrid.PBR.ImageBasedLighting
{
    public class ImageBasedLightingUnlitMaterial : IMaterial
    {
        private readonly UnlitMaterial _material;
        private readonly SimpleUniformPool<UnlitMaterialArguments> _uniformPool;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly uint _offset;

        public ImageBasedLightingUnlitMaterial(UnlitMaterial material,
            SimpleUniformPool<UnlitMaterialArguments> uniformPool, GraphicsDevice graphicsDevice,
            ResourceCache resourceCache)
        {
            _material = material;
            _uniformPool = uniformPool;
            _graphicsDevice = graphicsDevice;
            _offset = _uniformPool.Allocate();
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
                ResourceSet = new ResourceSetAndOffsets(graphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                    ResourceLayout,
                    uniformPool.BindableResource,
                    material.BaseColorMap.Map,
                    material.BaseColorMap.Sampler ?? _graphicsDevice.Aniso4xSampler)), _offset);
            }
            else
            {
                var resourceLayoutDescription = new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("UnlitMaterialArguments", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.DynamicBinding)
                );
                ResourceLayout = resourceCache.GetResourceLayout(resourceLayoutDescription);
                ResourceSet = new ResourceSetAndOffsets(graphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                    ResourceLayout,
                    uniformPool.BindableResource)), _offset);
            }

            Update();
        }

        public ResourceLayout ResourceLayout { get; }

        public ResourceSetAndOffsets ResourceSet { get; }

        public UnlitShaderFlags ShaderFlags { get; }

        public void Dispose()
        {
            ResourceSet.ResourceSet?.Dispose();
            _uniformPool.Release(_offset);
        }

        public void Update()
        {
            var args = new UnlitMaterialArguments
            {
                BaseColorFactor = _material.BaseColorFactor,
                AlphaCutoff = _material.AlphaCutoff,
                BaseColorMapUV = _material.BaseColorMap.UV
            };
            _uniformPool.UpdateBuffer(_offset, ref args);
        }
    }
}