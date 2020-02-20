using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid.PBR.DataStructures;
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

            ModelViewProjectionResourceLayout = ResourceCache.GetResourceLayout(ProjViewModelLayoutDescription);
            ModelViewProjectionResourceSet = ResourceCache.GetResourceSet(new ResourceSetDescription(
                ModelViewProjectionResourceLayout, _projViewBuffer,
                _nodeProperties.BindableResource));

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

        public IMaterialBinding<ImageBasedLightingPasses> BindMaterial(IMaterial unlitMaterial,
            PrimitiveTopology topology,
            uint indexCount,
            uint nodeUniformOffset,
            VertexLayoutDescription vertexLayoutDescription)
        {
            if (unlitMaterial is ImageBasedLightingUnlitMaterial imageBasedLightingUnlitMaterial)
                return _unlitTechnique.BindMaterial(imageBasedLightingUnlitMaterial, topology, indexCount,
                    nodeUniformOffset,
                    vertexLayoutDescription);
            return null;
        }

        public void Dispose()
        {
            _projViewBuffer.Dispose();
        }

        public IMaterial CreateMaterial(UnlitMaterial unlitMaterial)
        {
            return new ImageBasedLightingUnlitMaterial(unlitMaterial, _unlitArgumentsPool, _graphicsDevice,
                ResourceCache);
        }
    }
}