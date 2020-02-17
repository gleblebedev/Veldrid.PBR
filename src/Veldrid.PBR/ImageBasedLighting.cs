using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid.PBR.DataStructures;

namespace Veldrid.PBR
{
    public class ImageBasedLighting : IRenderPipeline<ImageBasedLightingPasses>
    {
        public static readonly ResourceLayoutDescription ProjViewModelLayoutDescription = new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("ViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("Model", ResourceKind.UniformBuffer, ShaderStages.Vertex,
                ResourceLayoutElementOptions.DynamicBinding));

        private readonly ResourceLayout _projViewModelLayout;
        private readonly IUniformPool<NodeProperties> _nodeProperties;

        private readonly ResourceLayout[] _opaquePassResourceLayouts;
        private readonly DeviceBuffer _projViewBuffer;

        public ImageBasedLighting(ResourceCache resourceCache, OutputDescription outputDescription,
            IUniformPool<NodeProperties> nodeProperties)
        {
            _nodeProperties = nodeProperties;
            ResourceCache = resourceCache;
            OutputDescription = outputDescription;

            _projViewBuffer = ResourceCache.ResourceFactory.CreateBuffer(
                new BufferDescription((uint) Marshal.SizeOf<ViewProjection>(),
                    BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            _projViewModelLayout = ResourceCache.GetResourceLayout(ProjViewModelLayoutDescription);
            ResourceSet = ResourceCache.GetResourceSet(new ResourceSetDescription(_projViewModelLayout, _projViewBuffer,
                _nodeProperties.BindableResource));
            _opaquePassResourceLayouts = new[] {ResourceCache.GetResourceLayout(ProjViewModelLayoutDescription)};
        }

        public ResourceCache ResourceCache { get; }

        public OutputDescription OutputDescription { get; }

        public ResourceSet ResourceSet { get; }

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
    }
}