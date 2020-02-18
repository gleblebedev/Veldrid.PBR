using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid.PBR.DataStructures;
using Veldrid.PBR.Unlit;

namespace Veldrid.PBR
{
    public class ImageBasedLighting : IRenderPipeline<ImageBasedLightingPasses>
    {
        public static readonly ResourceLayoutDescription ProjViewModelLayoutDescription = new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("ViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("Model", ResourceKind.UniformBuffer, ShaderStages.Vertex,
                ResourceLayoutElementOptions.DynamicBinding));
        public static readonly ResourceLayoutDescription UnlitMaterialArgumentsDescription = new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("UnlitMaterialArguments", ResourceKind.UniformBuffer, ShaderStages.Vertex, ResourceLayoutElementOptions.DynamicBinding));

        private readonly ResourceLayout _projViewModelLayout;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly IUniformPool<NodeProperties> _nodeProperties;

        private readonly ResourceLayout[] _opaquePassResourceLayouts;
        private readonly DeviceBuffer _projViewBuffer;

        private UnlitShaderFactory _unlitShaderFactory;
        private UnlitTechnique _unlitTechnique;
        private SimpleUniformPool<UnlitMaterialArguments> _unlitArgumentsPool;
        private ResourceLayout _unlitMaterialArgumentsLayout;

        public ImageBasedLighting(GraphicsDevice graphicsDevice, ResourceCache resourceCache, OutputDescription outputDescription,
            IUniformPool<NodeProperties> nodeProperties)
        {
            _graphicsDevice = graphicsDevice;
            _nodeProperties = nodeProperties;
            ResourceCache = resourceCache;
            OutputDescription = outputDescription;

            _projViewBuffer = ResourceCache.ResourceFactory.CreateBuffer(
                new BufferDescription((uint) Marshal.SizeOf<ViewProjection>(),
                    BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _unlitArgumentsPool = new SimpleUniformPool<UnlitMaterialArguments>((uint)1024, _graphicsDevice);

            _projViewModelLayout = ResourceCache.GetResourceLayout(ProjViewModelLayoutDescription);
            _unlitMaterialArgumentsLayout = ResourceCache.GetResourceLayout(UnlitMaterialArgumentsDescription);
            ModelViewProjectionResourceSet = ResourceCache.GetResourceSet(new ResourceSetDescription(_projViewModelLayout, _projViewBuffer,
                _nodeProperties.BindableResource));
            UnlitMaterialResourceSet = ResourceCache.GetResourceSet(new ResourceSetDescription(_unlitMaterialArgumentsLayout, _unlitArgumentsPool.BindableResource));
            _opaquePassResourceLayouts = new[] { _projViewModelLayout, _unlitMaterialArgumentsLayout };

            _unlitShaderFactory = new UnlitShaderFactory(_graphicsDevice.ResourceFactory);
            _unlitTechnique = new UnlitTechnique(_unlitShaderFactory, this);
        }

        public ResourceCache ResourceCache { get; }

        public OutputDescription OutputDescription { get; }

        public ResourceSet ModelViewProjectionResourceSet { get; }
        public ResourceSet UnlitMaterialResourceSet { get; }

        public void UpdateViewProjection(CommandList commandList, ref Matrix4x4 projection, ref Matrix4x4 view)
        {
            var data = new ViewProjection();
            data.View = view;
            data.Projection = projection;
            commandList.UpdateBuffer(_projViewBuffer, 0, ref data);
        }

        public void Dispose()
        {
            _projViewBuffer.Dispose();
        }

        public ResourceLayout[] GetResourceLayouts(ImageBasedLightingPasses pass)
        {
            switch (pass)
            {
                case ImageBasedLightingPasses.Opaque:
                    return _opaquePassResourceLayouts;
            }

            throw new IndexOutOfRangeException();
        }

        public IMaterial CreateMaterial(UnlitMaterial unlitMaterial)
        {
            return new ImageBasedLightingUnlitMaterial(unlitMaterial, _unlitArgumentsPool, _graphicsDevice);
        }

        public IMaterialBinding<ImageBasedLightingPasses> BindMaterial(IMaterial unlitMaterial, 
            PrimitiveTopology topology, 
            uint indexCount, 
            uint nodeUniformOffset, 
            VertexLayoutDescription vertexLayoutDescription)
        {
            if (unlitMaterial is ImageBasedLightingUnlitMaterial imageBasedLightingUnlitMaterial)
            {
                return _unlitTechnique.BindMaterial(imageBasedLightingUnlitMaterial.UniformOffset, topology, indexCount,
                    nodeUniformOffset,
                    vertexLayoutDescription);
            }
            return null;
        }
    }
}