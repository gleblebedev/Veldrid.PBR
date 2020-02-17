using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid.PBR.DataStructures;

namespace Veldrid.PBR
{
    public class ImageBasedLighting: IRenderPipeline<ImageBasedLightingPasses>
    {
        public static readonly ResourceLayoutDescription ProjViewModelLayoutDescription = new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("ViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("Model", ResourceKind.UniformBuffer, ShaderStages.Vertex, ResourceLayoutElementOptions.DynamicBinding));

        private readonly ResourceCache _resourceCache;
        private readonly OutputDescription _outputDescription;
        private IUniformPool<NodeProperties> _nodeProperties;
        private readonly ResourceSet _projViewModelResourceSet;
        private readonly ResourceLayout _projViewModelLayout;

        public ImageBasedLighting(ResourceCache resourceCache, OutputDescription outputDescription, IUniformPool<NodeProperties> nodeProperties)
        {
            _nodeProperties = nodeProperties;
            _resourceCache = resourceCache;
            _outputDescription = outputDescription;

            _projViewBuffer = _resourceCache.ResourceFactory.CreateBuffer(new BufferDescription((uint)Marshal.SizeOf<ViewProjection>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            _projViewModelLayout = _resourceCache.GetResourceLayout(ProjViewModelLayoutDescription);
            _projViewModelResourceSet = _resourceCache.GetResourceSet(new ResourceSetDescription(_projViewModelLayout, _projViewBuffer, _nodeProperties.BindableResource));
            _opaquePassResourceLayouts = new []{ _resourceCache.GetResourceLayout(ProjViewModelLayoutDescription) };
        }

        public ResourceCache ResourceCache => _resourceCache;
        public OutputDescription OutputDescription => _outputDescription;
        public ResourceSet ResourceSet => _projViewModelResourceSet;

        private ResourceLayout[] _opaquePassResourceLayouts;
        private DeviceBuffer _projViewBuffer;

        public ResourceLayout[] GetResourceLayouts(ImageBasedLightingPasses pass)
        {
            switch (pass)
            {
                case ImageBasedLightingPasses.Opaque:
                    return _opaquePassResourceLayouts;
            }
            throw new IndexOutOfRangeException();
        }

        public void Dispose()
        {
            _projViewBuffer.Dispose();
        }

        public void UpdateViewProjection(CommandList commandList, ref Matrix4x4 projection, ref Matrix4x4 view)
        {
            var data = new ViewProjection();
            data.View = view;
            data.Projection = projection;
            commandList.UpdateBuffer(_projViewBuffer, 0, ref data);
        }
    }
}