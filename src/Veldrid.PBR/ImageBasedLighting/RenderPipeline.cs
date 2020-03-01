using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid.PBR.Uniforms;
using Veldrid.PBR.Unlit;

namespace Veldrid.PBR.ImageBasedLighting
{
    public class RenderPipeline : IRenderPipeline<ImageBasedLightingPasses>
    {
        public static readonly ResourceLayoutDescription ProjViewModelLayoutDescription = new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("ViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("Model", ResourceKind.UniformBuffer, ShaderStages.Vertex,
                ResourceLayoutElementOptions.DynamicBinding));

        private readonly GraphicsDevice _graphicsDevice;
        private readonly IUniformPool<NodeProperties> _nodeProperties;

        private readonly DeviceBuffer _projViewBuffer;

        private readonly UnlitShaderFactory _unlitShaderFactory;
        private readonly UnlitTechnique _unlitTechnique;
        private readonly SimpleUniformPool<UnlitMaterialArguments> _unlitArgumentsPool;
        private readonly SimpleUniformPool<MetallicRoughnessMaterialArguments> _metallicRoughnessArgumentsPool;
        private readonly SimpleUniformPool<SpecularGlossinessMaterialArguments> _specularGlossinessArgumentsPool;

        public RenderPipeline(GraphicsDevice graphicsDevice, ResourceCache resourceCache,
            OutputDescription outputDescription,
            IUniformPool<NodeProperties> nodeProperties)
        {
            _graphicsDevice = graphicsDevice;
            _nodeProperties = nodeProperties;
            ResourceCache = resourceCache;
            OutputDescription = outputDescription;

            _projViewBuffer = ResourceCache.ResourceFactory.CreateBuffer(
                new BufferDescription((uint) Marshal.SizeOf<ViewProjection>(),
                    BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _unlitArgumentsPool = new SimpleUniformPool<UnlitMaterialArguments>(1024, _graphicsDevice);
            _metallicRoughnessArgumentsPool = new SimpleUniformPool<MetallicRoughnessMaterialArguments>(1024, _graphicsDevice);
            _specularGlossinessArgumentsPool = new SimpleUniformPool<SpecularGlossinessMaterialArguments>(1024, _graphicsDevice);

            ModelViewProjectionResourceLayout = ResourceCache.GetResourceLayout(ProjViewModelLayoutDescription);
            var resourceSetDescription = new ResourceSetDescription(
                ModelViewProjectionResourceLayout,
                _projViewBuffer,
                _nodeProperties.BindableResource);
            ModelViewProjectionResourceSet = ResourceCache.GetResourceSet(resourceSetDescription);

            _unlitShaderFactory = new UnlitShaderFactory(_graphicsDevice.ResourceFactory);
            _unlitTechnique = new UnlitTechnique(_unlitShaderFactory, this);
        }

        public ResourceCache ResourceCache { get; }

        public OutputDescription OutputDescription { get; }

        public ResourceSet ModelViewProjectionResourceSet { get; }

        public ResourceLayout ModelViewProjectionResourceLayout { get; }

        public void UpdateViewProjection(CommandList commandList, ref Matrix4x4 projection, ref Matrix4x4 view)
        {
            var data = new ViewProjection();
            data.View = view;
            data.Projection = projection;
            commandList.UpdateBuffer(_projViewBuffer, 0, ref data);
        }

        public IMaterialBinding<ImageBasedLightingPasses> BindMaterial(IMaterial unlitMaterial, ref PrimitiveDrawCall drawCall)
        {
            if (unlitMaterial is UnlitMaterialBinding imageBasedLightingUnlitMaterial)
                return _unlitTechnique.BindMaterial(imageBasedLightingUnlitMaterial, ref drawCall);
            return null;
        }

        public void Dispose()
        {
            _projViewBuffer.Dispose();
        }

        public IMaterial CreateMaterial(MaterialBase material)
        {
            if (material is UnlitMaterial unlitMaterial)
            {
                return new UnlitMaterialBinding(unlitMaterial, _unlitArgumentsPool, _graphicsDevice, ResourceCache);
            }
            else if (material is MetallicRoughness metallicRoughness)
            {
                return new MetallicRoughnessMaterialBinding(metallicRoughness, _metallicRoughnessArgumentsPool, _graphicsDevice, ResourceCache);
            }
            else if (material is SpecularGlossiness specularGlossiness)
            {
                return new SpecularGlossinessMaterialBinding(specularGlossiness, _specularGlossinessArgumentsPool, _graphicsDevice, ResourceCache);
            }
            throw new ArgumentException("Unrecognized material. It should be one of the following: "+nameof(UnlitMaterial)+", " + nameof(MetallicRoughness) + ", " + nameof(SpecularGlossiness));
        }
    }
}