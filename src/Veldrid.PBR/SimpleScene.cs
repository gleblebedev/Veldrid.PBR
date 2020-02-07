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

layout(location = 0) in vec4 fsin_color;
layout(location = 0) out vec4 fsout_color;

void main()
{
    fsout_color = fsin_color;
}";

        public const string VertexShader = @"
#version 450

layout(set = 0, binding = 0) uniform ViewProjection
{
    mat4 View;
    mat4 Projection;
};
layout(set = 0, binding = 1) uniform ModelBuffer
{
    mat4 Model;
};

layout(location = 0) in vec3 NORMAL;
layout(location = 1) in vec3 POSITION;
layout(location = 2) in vec4 TANGENT;
layout(location = 3) in vec2 TEXCOORD_0;

layout(location = 0) out vec4 fsin_color;

void main()
{
    vec4 worldPosition = Model * vec4(POSITION, 1);
    vec4 viewPosition = View * worldPosition;
    vec4 clipPosition = Projection * viewPosition;
    fsin_color = vec4(NORMAL,1);
    gl_Position = clipPosition;
}";

        public static readonly ResourceLayoutDescription ProjViewModelLayoutDescription = new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("ViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("Model", ResourceKind.UniformBuffer, ShaderStages.Vertex));

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
        private readonly DeviceBuffer _modelBuffer;
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

            _projViewBuffer = ResourceFactory.CreateBuffer(new BufferDescription(2 * 64,
                BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _modelBuffer = ResourceFactory.CreateBuffer(new BufferDescription(64,
                BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            _projViewModelLayout = _resourceCache.GetResourceLayout(ProjViewModelLayoutDescription);
            _projViewModelResourceSet =
                _resourceCache.GetResourceSet(new ResourceSetDescription(_projViewModelLayout, _projViewBuffer,
                    _modelBuffer));

            _buffers = new List<DeviceBuffer>(_content.NumBuffers);

            for (var index = 0; index < _content.NumBuffers; index++)
            {
                var deviceBuffer = _content.CreateBuffer(index, graphicsDevice, ResourceFactory);
                _buffers.Add(deviceBuffer);
                _disposables.Add(deviceBuffer);
            }

            var vertexLayouts = new[]
            {
                _content.GetVertexLayoutDescription(_content.GetVertexBufferView(0).Elements)
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
            _cl.ClearColorTarget(0, RgbaFloat.Blue);

            var viewProj = new ViewProjection();
            viewProj.Projection = Matrix4x4.CreatePerspectiveFieldOfView(3.14f * 0.5f, 1, 0.1f, 100.0f);

            viewProj.View =
                Matrix4x4.CreateLookAt(new Vector3((float) Math.Cos(_angle), 1, (float) Math.Sin(_angle)) * 3,
                    Vector3.Zero, Vector3.UnitY);
            _cl.UpdateBuffer(_projViewBuffer, 0, ref viewProj);


            for (var index = 0; index < _content.NumNodes; index++)
            {
                ref var node = ref _content.GetNode(index);
                if (node.MeshIndex < 0) continue;


                _cl.UpdateBuffer(_modelBuffer, 0, ref node.WorldTransform);

                ref var mesh = ref _content.GetMesh(node.MeshIndex);
                foreach (var primitiveIndex in mesh.Primitives)
                {
                    ref var primitive = ref _content.GetPrimitive(primitiveIndex);
                    var vbView = _content.GetVertexBufferView(primitive.VertexBufferView);
                    _cl.SetVertexBuffer(0, _buffers[vbView.Buffer], vbView.Offset);
                    _cl.SetIndexBuffer(_buffers[primitive.IndexBuffer], primitive.IndexBufferFormat,
                        primitive.IndexBufferOffset);
                    _cl.SetPipeline(_pipelines[0]);
                    _cl.SetGraphicsResourceSet(0, _projViewModelResourceSet);
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