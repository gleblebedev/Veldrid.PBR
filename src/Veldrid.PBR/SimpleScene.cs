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
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Swapchain _swapchain;
        private readonly ResourceCache _resourceCache;
        private readonly PbrContent _content;
        private readonly CommandList _cl;
        private readonly List<IDisposable> _disposables;
        private readonly List<DeviceBuffer> _buffers;
        private readonly UnlitShaderFactory _unlitShaderFactory;
        private readonly UnlitTechnique _unlitTechnique;
        private float _angle;
        private readonly SimpleNode[] _nodes;
        private readonly ImageBasedLighting _renderPipeline;
        private readonly SimpleUniformPool<NodeProperties> _nodeProperties;
        private readonly float _radius;
        private readonly Vector3 _center;

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
                _nodeProperties = new SimpleUniformPool<NodeProperties>((uint) _content.NumNodes, _graphicsDevice);
                _disposables.Add(_nodeProperties);
            }
            {
                _renderPipeline = new ImageBasedLighting(_resourceCache, _swapchain.Framebuffer.OutputDescription,
                    _nodeProperties);
                _disposables.Add(_renderPipeline);
            }
            {
                _unlitShaderFactory = new UnlitShaderFactory(_graphicsDevice.ResourceFactory);
                var simpleUniformPool =
                    new SimpleUniformPool<UnlitMaterialArguments>((uint) content.NumUnlitMaterials, _graphicsDevice);
                _disposables.Add(simpleUniformPool);
                _unlitTechnique = new UnlitTechnique(simpleUniformPool, _unlitShaderFactory, _renderPipeline);
            }
            _disposables.Add(_unlitShaderFactory);
            _disposables.Add(_cl);


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

            _radius = 0.01f;
            _nodes = new SimpleNode[_content.NumNodes];
            var sceneMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var sceneMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (var index = 0; index < _content.NumNodes; index++)
            {
                var simpleNode = new SimpleNode();
                var nodeData = _content.GetNode(index);

                var worldTransform = nodeData.WorldTransform;
                simpleNode.NodeData = nodeData;
                {
                    simpleNode.ModelUniformOffset = _nodeProperties.Allocate();
                    var nodeProperties = new NodeProperties {WorldTransform = worldTransform};
                    _nodeProperties.UpdateBuffer(simpleNode.ModelUniformOffset, ref nodeProperties);
                }
                if (nodeData.MeshIndex >= 0)
                {
                    simpleNode.MeshIndex = nodeData.MeshIndex;
                    ref var meshData = ref _content.GetMesh(nodeData.MeshIndex);
                    var bindings =
                        new IMaterialBinding<ImageBasedLightingPasses>[Math.Min(nodeData.MaterialBindings.Count,
                            meshData.Primitives.Count)];
                    for (var primitiveIndex = 0; primitiveIndex < bindings.Length; ++primitiveIndex)
                    {
                        ref var primitive = ref _content.GetPrimitive(meshData.Primitives[primitiveIndex]);

                        var min = Vector3.Transform(primitive.BoundingBoxMin, worldTransform);
                        var max = Vector3.Transform(primitive.BoundingBoxMax, worldTransform);
                        foreach (var v in GetBoundingBoxCorners(min, max))
                        {
                            sceneMin = Vector3.Min(sceneMin, v);
                            sceneMax = Vector3.Max(sceneMax, v);
                        }

                        var materialReference =
                            _content.GetMaterialReference(nodeData.MaterialBindings[primitiveIndex]);
                        var vertexLayoutDescription =
                            _content.GetVertexLayoutDescription(_content.GetVertexBufferView(primitive.VertexBufferView)
                                .Elements);
                        switch (materialReference.MaterialType)
                        {
                            case MaterialType.Unlit:
                                bindings[primitiveIndex] = _unlitTechnique.BindMaterial(
                                    unlitMaterials[materialReference.Material], primitive.PrimitiveTopology,
                                    primitive.IndexCount, simpleNode.ModelUniformOffset, vertexLayoutDescription);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }

                    simpleNode.MaterialBindings = bindings;
                }

                _nodes[index] = simpleNode;
            }

            _center = (sceneMax + sceneMin) * 0.5f;
            _radius = (sceneMax - sceneMin).Length() * 0.5f;
        }

        public ResourceFactory ResourceFactory { get; }

        public void Render(float deltaSeconds)
        {
            _angle += deltaSeconds * 0.4f;
            _cl.Begin();
            _cl.SetFramebuffer(_swapchain.Framebuffer);
            _cl.SetFullViewport(0);
            _cl.ClearDepthStencil(1.0f);
            _cl.ClearColorTarget(0, new RgbaFloat(0, 0.1f, 0.5f, 1));

            var radius = _radius * 2;
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(3.14f * 0.5f,
                _swapchain.Framebuffer.Width / (float) _swapchain.Framebuffer.Height, 0.1f, radius * 4.0f);
            var from = new Vector3((float) Math.Cos(_angle), 1, (float) Math.Sin(_angle)) * radius;
            var view = Matrix4x4.CreateLookAt(from + _center, _center, Vector3.UnitY);
            _renderPipeline.UpdateViewProjection(_cl, ref projection, ref view);

            for (var index = 0; index < _nodes.Length; index++)
            {
                var simpleNode = _nodes[index];
                //var node = simpleNode.NodeData;

                if (simpleNode.MaterialBindings == null) continue;
                for (var i = 0; i < simpleNode.MaterialBindings.Length; i++)
                {
                    var materialBinding = simpleNode.MaterialBindings[i];
                    if (materialBinding != null)
                    {
                        var passBinding = materialBinding[ImageBasedLightingPasses.Opaque];
                        if (passBinding != null)
                        {
                            ref var mesh = ref _content.GetMesh(simpleNode.MeshIndex);

                            foreach (var primitiveIndex in mesh.Primitives)
                            {
                                ref var primitive = ref _content.GetPrimitive(primitiveIndex);
                                var vbView = _content.GetVertexBufferView(primitive.VertexBufferView);
                                _cl.SetVertexBuffer(0, _buffers[vbView.Buffer], vbView.Offset);
                                _cl.SetIndexBuffer(_buffers[primitive.IndexBuffer], primitive.IndexBufferFormat,
                                    primitive.IndexBufferOffset);
                                passBinding.Draw(_cl);
                            }
                        }
                    }
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

        private IEnumerable<Vector3> GetBoundingBoxCorners(Vector3 min, Vector3 max)
        {
            yield return new Vector3(min.X, min.Y, min.Z);
            yield return new Vector3(min.X, min.Y, max.Z);
            yield return new Vector3(min.X, max.Y, min.Z);
            yield return new Vector3(min.X, max.Y, max.Z);
            yield return new Vector3(max.X, min.Y, min.Z);
            yield return new Vector3(max.X, min.Y, max.Z);
            yield return new Vector3(max.X, max.Y, min.Z);
            yield return new Vector3(max.X, max.Y, max.Z);
        }

        public class SimpleNode
        {
            public NodeData NodeData;
            public int MeshIndex;
            public IMaterialBinding<ImageBasedLightingPasses>[] MaterialBindings;
            public uint ModelUniformOffset;
        }
    }
}