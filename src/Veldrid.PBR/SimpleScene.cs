using System;
using System.Collections.Generic;
using System.Numerics;
using Veldrid.PBR.BinaryData;
using Veldrid.PBR.DataStructures;
using Veldrid.PBR.Unlit;

namespace Veldrid.PBR
{
    // Non-thread-safe cache for resources.
    public class SimpleScene : IDisposable
    {
        public class SimpleNode
        {
            public uint UniformOffset;
            public NodeData NodeData;
        }
        public static readonly ResourceLayoutDescription ProjViewModelLayoutDescription = new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("ViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("Model", ResourceKind.UniformBuffer, ShaderStages.Vertex, ResourceLayoutElementOptions.DynamicBinding));

        private readonly GraphicsDevice _graphicsDevice;
        private readonly Swapchain _swapchain;
        private readonly ResourceCache _resourceCache;
        private readonly PbrContent _content;
        private readonly CommandList _cl;
        private readonly List<IDisposable> _disposables;
        private readonly List<DeviceBuffer> _buffers;
        private readonly List<Pipeline> _pipelines;
        private readonly ResourceSet _projViewModelResourceSet;
        private readonly ResourceLayout _projViewModelLayout;
        private readonly DeviceBuffer _projViewBuffer;
        private float _angle;
        private readonly UnlitShaderFactory _unlitShaderFactory;
        private readonly UnlitTechnique _unlitTechnique;
        private SimpleNode[] _nodes;
        private SimpleUniformPool<NodeProperties> _nodeProperties;

        public SimpleScene(GraphicsDevice graphicsDevice, Swapchain swapchain, ResourceCache resourceCache,
            PbrContent content)
        {
            _graphicsDevice = graphicsDevice;
            _swapchain = swapchain;
            _resourceCache = resourceCache;
            _content = content;
            ResourceFactory = graphicsDevice.ResourceFactory;
            _disposables = new List<IDisposable>();
            _disposables.Add(content);
            _cl = ResourceFactory.CreateCommandList();
            {
                _unlitShaderFactory = new UnlitShaderFactory(_graphicsDevice.ResourceFactory);
                var simpleUniformPool =
                    new SimpleUniformPool<UnlitMaterialArguments>((uint) content.NumUnlitMaterials, _graphicsDevice);
                _disposables.Add(simpleUniformPool);
                _unlitTechnique = new UnlitTechnique(simpleUniformPool);
            }
            {
                _nodeProperties = new SimpleUniformPool<NodeProperties>((uint)_content.NumNodes, _graphicsDevice);
                _disposables.Add(_nodeProperties);
            }
            _disposables.Add(_unlitShaderFactory);
            _disposables.Add(_cl);

            _projViewBuffer = ResourceFactory.CreateBuffer(new BufferDescription(2 * 64,
                BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            _projViewModelLayout = _resourceCache.GetResourceLayout(ProjViewModelLayoutDescription);
            _projViewModelResourceSet = _resourceCache.GetResourceSet(new ResourceSetDescription(_projViewModelLayout, _projViewBuffer, _nodeProperties.BindableResource));

            _buffers = new List<DeviceBuffer>(_content.NumBuffers);

            for (var index = 0; index < _content.NumBuffers; index++)
            {
                var deviceBuffer = _content.CreateBuffer(index, _graphicsDevice, ResourceFactory);
                _buffers.Add(deviceBuffer);
                _disposables.Add(deviceBuffer);
            }

            var unlitMaterials = new UnlitMaterial[_content.NumUnlitMaterials];
            for (var index = 0; index < _content.NumUnlitMaterials; index++)
            {
                var unlitMaterial = _content.CreateUnlitMaterial(index, _graphicsDevice, ResourceFactory);
                unlitMaterials[index] = unlitMaterial;
            }

            var vertexLayouts = new[]
            {
                _content.GetVertexLayoutDescription(_content.GetVertexBufferView(0).Elements)
            };

            _nodes = new SimpleNode[_content.NumNodes];
            for (var index = 0; index < _content.NumNodes; index++)
            {
                var simpleNode = new SimpleNode();
                var nodeData = _content.GetNode(index);

                simpleNode.NodeData = nodeData;
                simpleNode.UniformOffset = _nodeProperties.Allocate();
                var nodeProperties = new NodeProperties(){ WorldTransform = nodeData.WorldTransform };
                _nodeProperties.UpdateBuffer(simpleNode.UniformOffset, ref nodeProperties);
                _nodes[index] = simpleNode;
            }

            var shaders = _unlitShaderFactory.GetOrCreateShaders(new UnlitShaderKey {Elements = vertexLayouts[0]});

            _pipelines = new List<Pipeline>(1);
            //foreach (var pipelineData in _content.Pipelines)
            {
                var description = new GraphicsPipelineDescription();
                description.BlendState = BlendStateDescription.SingleOverrideBlend;
                description.DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual;
                description.PrimitiveTopology = PrimitiveTopology.TriangleList;
                description.RasterizerState = new RasterizerStateDescription
                {
                    CullMode = FaceCullMode.None,
                    FillMode = PolygonFillMode.Solid,
                    FrontFace = FrontFace.Clockwise,
                    DepthClipEnabled = true,
                    ScissorTestEnabled = false
                };
                description.ShaderSet = new ShaderSetDescription(vertexLayouts, shaders);
                description.ResourceLayouts = new[] {_projViewModelLayout};
                description.Outputs = swapchain.Framebuffer.OutputDescription;
                var pipeline = _resourceCache.GetPipeline(ref description);
                _pipelines.Add(pipeline);
            }
        }

        public ResourceFactory ResourceFactory { get; }

        public void Render(float deltaSeconds)
        {
            _angle += deltaSeconds * 0.1f;
            _cl.Begin();
            _cl.SetFramebuffer(_swapchain.Framebuffer);
            _cl.SetFullViewport(0);
            _cl.ClearDepthStencil(1.0f);
            _cl.ClearColorTarget(0, new RgbaFloat(0, 0.1f, 0.5f, 1));

            var viewProj = new ViewProjection();
            viewProj.Projection = Matrix4x4.CreatePerspectiveFieldOfView(3.14f * 0.5f, 1, 0.1f, 100.0f);

            viewProj.View =
                Matrix4x4.CreateLookAt(new Vector3((float) Math.Cos(_angle), 1, (float) Math.Sin(_angle)) * 3,
                    Vector3.Zero, Vector3.UnitY);
            _cl.UpdateBuffer(_projViewBuffer, 0, ref viewProj);


            for (var index = 0; index < _nodes.Length; index++)
            {
                var simpleNode = _nodes[index];
                var node = simpleNode.NodeData;
                
                if (node.MeshIndex < 0) continue;

                ref var mesh = ref _content.GetMesh(node.MeshIndex);
                foreach (var primitiveIndex in mesh.Primitives)
                {
                    ref var primitive = ref _content.GetPrimitive(primitiveIndex);
                    var vbView = _content.GetVertexBufferView(primitive.VertexBufferView);
                    _cl.SetVertexBuffer(0, _buffers[vbView.Buffer], vbView.Offset);
                    _cl.SetIndexBuffer(_buffers[primitive.IndexBuffer], primitive.IndexBufferFormat,
                        primitive.IndexBufferOffset);
                    _cl.SetPipeline(_pipelines[0]);
                    _cl.SetGraphicsResourceSet(0, _projViewModelResourceSet, 1, ref simpleNode.UniformOffset);
                    _cl.DrawIndexed(primitive.IndexCount, 1, 0, 0, 0);
                }
            }


            _cl.End();
            _graphicsDevice.SubmitCommands(_cl);
            _graphicsDevice.SwapBuffers(_swapchain);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables) _cl.Dispose();
        }
    }
}