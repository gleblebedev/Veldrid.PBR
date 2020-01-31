using System;
using System.Text;
using System.Collections.Generic;
using System.Numerics;
using Veldrid.SPIRV;

namespace Veldrid.PBR
{
    // Non-thread-safe cache for resources.
    public class SimpleScene : IDisposable
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Swapchain _swapchain;
        private readonly ResourceCache _resourceCache;
        private readonly PbrContent _content;
        private readonly CommandList _cl;
        private readonly List<IDisposable> _disposables;
        private readonly List<DeviceBuffer> _buffers;
        private readonly List<Pipeline> _pipelines;
        private ResourceSet _mainProjViewRS;
        private ResourceLayout _mainProjViewLayout;
        private DeviceBuffer _projViewBuffer;

        public SimpleScene(GraphicsDevice graphicsDevice, Swapchain swapchain, ResourceCache resourceCache, PbrContent content)
        {
            _graphicsDevice = graphicsDevice;
            _swapchain = swapchain;
            _resourceCache = resourceCache;
            _content = content;
            ResourceFactory = graphicsDevice.ResourceFactory;
            _disposables = new List<IDisposable>();
            _cl = ResourceFactory.CreateCommandList();
            _disposables.Add(_cl);

            _projViewBuffer = ResourceFactory.CreateBuffer(new BufferDescription(2*64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            _mainProjViewLayout = _resourceCache.GetResourceLayout(ResourceCache.ProjViewLayoutDescription);
            _mainProjViewRS = _resourceCache.GetResourceSet(new ResourceSetDescription(_mainProjViewLayout, _projViewBuffer));

            _buffers = new List<DeviceBuffer>(content.Buffers.Count);
            foreach (var vertexBuffer in content.Buffers)
            {
                var deviceBuffer = ResourceFactory.CreateBuffer(vertexBuffer.Description);
                _buffers.Add(deviceBuffer);
                _disposables.Add(deviceBuffer);
                _graphicsDevice.UpdateBuffer(deviceBuffer, 0, vertexBuffer.Data);
            }
            var vertexLayouts = new VertexLayoutDescription[] { new VertexLayoutDescription(new VertexElementDescription("POSITION", VertexElementFormat.Float3, VertexElementSemantic.TextureCoordinate)), };
            var shaders = ResourceFactory.CreateFromSpirv(
                new ShaderDescription(ShaderStages.Vertex, Encoding.ASCII.GetBytes(VertexShader), "main"),
                new ShaderDescription(ShaderStages.Fragment, Encoding.ASCII.GetBytes(FragmentShader), "main"));

            _pipelines = new List<Pipeline>(content.Pipelines.Count);
            foreach (var pipelineData in content.Pipelines)
            {
                var description = new GraphicsPipelineDescription();
                description.BlendState = pipelineData.BlendState;
                description.DepthStencilState = pipelineData.DepthStencilState;
                description.PrimitiveTopology = pipelineData.PrimitiveTopology;
                description.RasterizerState = pipelineData.RasterizerState;
                description.ShaderSet = new ShaderSetDescription(vertexLayouts, shaders);
                description.ResourceLayouts = new ResourceLayout[]{ _mainProjViewLayout };
                description.Outputs = swapchain.Framebuffer.OutputDescription;
                var pipeline = _resourceCache.GetPipeline(ref description);
                _pipelines.Add(pipeline);
            }
        }

        public const string FragmentShader = @"
";
        public const string VertexShader = @"
";

        public ResourceFactory ResourceFactory { get; }

        public void Render(float deltaSeconds)
        {
            _cl.Begin();
            _cl.SetFramebuffer(_swapchain.Framebuffer);
            _cl.SetFullViewport(0);
            _cl.ClearColorTarget(0, RgbaFloat.Blue);

            var viewProj = new ViewProjection();
            viewProj.Projection = Matrix4x4.CreatePerspectiveFieldOfView(3.14f*0.5f, 1, 0.1f, 100.0f);
            viewProj.View = Matrix4x4.CreateLookAt(Vector3.One*10, Vector3.Zero, Vector3.UnitY);
            _cl.UpdateBuffer(_projViewBuffer, 0, ref viewProj);

            foreach (var mesh in _content.Meshes)
            {
                foreach (var primitive in mesh.Primitives.Enumerate(_content.Primitives))
                {
                    _cl.SetVertexBuffer(0, _buffers[primitive.VertexBuffer], primitive.VertexBufferOffset);
                    _cl.SetIndexBuffer(_buffers[primitive.IndexBuffer], primitive.IndexBufferFormat, primitive.IndexBufferOffset);
                    _cl.SetPipeline(_pipelines[primitive.Pipeline]);
                    _cl.SetGraphicsResourceSet(0, _mainProjViewRS);
                    _cl.DrawIndexed((uint)primitive.IndexCount, 1, 0, 0, 0);
                }
            }

            _cl.End();
            _graphicsDevice.SubmitCommands(_cl);
            _graphicsDevice.SwapBuffers(_swapchain);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                _cl.Dispose();
            }
        }
    }
}