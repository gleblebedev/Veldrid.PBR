using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Veldrid.SPIRV;

namespace Veldrid.PBR
{
    // Non-thread-safe cache for resources.
    public class SimpleScene : IDisposable
    {
        public const string FragmentShader = @"
#version 450

layout(location = 0) out vec4 fsout_color;

void main()
{
    fsout_color = vec4(1,1,1,1);
}";

        public const string VertexShader = @"
#version 450

layout(set = 0, binding = 0) uniform ViewProjection
{
    mat4 View;
    mat4 Projection;
};

layout(location = 0) in vec3 POSITION;

void main()
{
    vec4 worldPosition = vec4(POSITION, 1);
    vec4 viewPosition = View * worldPosition;
    vec4 clipPosition = Projection * viewPosition;
    gl_Position = clipPosition;
}";

        private readonly GraphicsDevice _graphicsDevice;
        private readonly Swapchain _swapchain;
        private readonly ResourceCache _resourceCache;
        private readonly PbrContent _content;
        private readonly CommandList _cl;
        private readonly List<IDisposable> _disposables;
        private readonly List<DeviceBuffer> _buffers;
        private readonly List<Pipeline> _pipelines;
        private readonly ResourceSet _mainProjViewRS;
        private readonly ResourceLayout _mainProjViewLayout;
        private readonly DeviceBuffer _projViewBuffer;
        private float _angle;

        public SimpleScene(GraphicsDevice graphicsDevice, Swapchain swapchain, ResourceCache resourceCache,
            PbrContent content)
        {
            _graphicsDevice = graphicsDevice;
            _swapchain = swapchain;
            _resourceCache = resourceCache;
            _content = content;
            ResourceFactory = graphicsDevice.ResourceFactory;
            _disposables = new List<IDisposable>();
            _cl = ResourceFactory.CreateCommandList();
            _disposables.Add(_cl);

            _projViewBuffer =
                ResourceFactory.CreateBuffer(new BufferDescription(2 * 64,
                    BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            _mainProjViewLayout = _resourceCache.GetResourceLayout(ResourceCache.ProjViewLayoutDescription);
            _mainProjViewRS =
                _resourceCache.GetResourceSet(new ResourceSetDescription(_mainProjViewLayout, _projViewBuffer));

            _buffers = new List<DeviceBuffer>(_content.NumBuffers);

            for (var index = 0; index < _content.NumBuffers; index++)
            {
                var deviceBuffer = _content.CreateBuffer(index, graphicsDevice, ResourceFactory);
                _buffers.Add(deviceBuffer);
                _disposables.Add(deviceBuffer);
            }

            var vertexLayouts = new[]
            {
                new VertexLayoutDescription(new VertexElementDescription("POSITION", VertexElementFormat.Float3,
                    VertexElementSemantic.TextureCoordinate))
            };
            var shaders = ResourceFactory.CreateFromSpirv(
                new ShaderDescription(ShaderStages.Vertex, Encoding.ASCII.GetBytes(VertexShader), "main"),
                new ShaderDescription(ShaderStages.Fragment, Encoding.ASCII.GetBytes(FragmentShader), "main"));

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
                    FillMode = PolygonFillMode.Wireframe,
                    FrontFace = FrontFace.Clockwise,
                    DepthClipEnabled = true,
                    ScissorTestEnabled = false
                };
                description.ShaderSet = new ShaderSetDescription(vertexLayouts, shaders);
                description.ResourceLayouts = new[] {_mainProjViewLayout};
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
            _cl.ClearColorTarget(0, RgbaFloat.Blue);

            var viewProj = new ViewProjection();
            viewProj.Projection = Matrix4x4.CreatePerspectiveFieldOfView(3.14f * 0.5f, 1, 0.1f, 100.0f);

            viewProj.View =
                Matrix4x4.CreateLookAt(new Vector3((float) Math.Cos(_angle), 1, (float) Math.Sin(_angle)) * 3,
                    Vector3.Zero, Vector3.UnitY);
            _cl.UpdateBuffer(_projViewBuffer, 0, ref viewProj);

            for (var index = 0; index < _content.NumMeshes; index++)
            {
                ref var mesh = ref _content.GetMesh(index);
                foreach (var primitiveIndex in mesh.Primitives)
                {
                    ref var primitive = ref _content.GetPrimitive(primitiveIndex);

                    _cl.SetVertexBuffer(0, _buffers[primitive.VertexBuffer], primitive.VertexBufferOffset);
                    _cl.SetIndexBuffer(_buffers[primitive.IndexBuffer], primitive.IndexBufferFormat,
                        primitive.IndexBufferOffset);
                    _cl.SetPipeline(_pipelines[0]);
                    _cl.SetGraphicsResourceSet(0, _mainProjViewRS);
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