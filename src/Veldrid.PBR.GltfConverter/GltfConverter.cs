using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpGLTF.Schema2;
using SharpGLTF.Validation;

namespace Veldrid.PBR
{
    public class GltfConverter
    {
        public MemoryStream _indexBuffer;
        public BinaryWriter _indexWriter;
        public MemoryStream _vertexBuffer;
        private readonly ModelRoot _modelRoot;
        private readonly BinaryWriter _vertexWriter;
        private readonly ContentToWrite _content;

        internal GltfConverter(ModelRoot modelRoot)
        {
            _modelRoot = modelRoot;
            _content = new ContentToWrite();
            _indexBuffer = new MemoryStream();
            _indexWriter = new BinaryWriter(_indexBuffer);
            _vertexBuffer = new MemoryStream();
            _vertexWriter = new BinaryWriter(_vertexBuffer);
        }

        public byte[] Content { get; private set; }

        public static byte[] ReadGtlf(string fileName)
        {
            var modelRoot = ModelRoot.Load(fileName, new ReadSettings {Validation = ValidationMode.Skip});
            var converter = new GltfConverter(modelRoot);
            converter.Convert();
            return converter.Content;
        }

        private void Convert()
        {
            var skinPerMesh = new Skin[_modelRoot.LogicalMeshes.Count];
            foreach (var node in _modelRoot.LogicalNodes)
                if (node.Skin != null && node.Mesh != null)
                    skinPerMesh[node.Mesh.LogicalIndex] = node.Skin;
            _content.VertexElements.Add(new VertexElementData(_content.AddString("POSITION"),
                VertexElementFormat.Float3, VertexElementSemantic.TextureCoordinate));
            foreach (var mesh in _modelRoot.LogicalMeshes) ConvertMesh(mesh, skinPerMesh[mesh.LogicalIndex]);
            _content.Buffers.Add(new BufferData(new BufferDescription(0, BufferUsage.VertexBuffer),
                _content.AddBlob(_vertexBuffer)));
            _content.Buffers.Add(new BufferData(new BufferDescription(0, BufferUsage.IndexBuffer),
                _content.AddBlob(_indexBuffer)));
            foreach (var texture in _modelRoot.LogicalTextures)
            {
                var textureData = new TextureData(_content.AddBlob(texture.PrimaryImage.GetImageContent()))
                {
                    Name = _content.AddString(texture.PrimaryImage.Name)
                };
                _content.Textures.Add(textureData);
            }

            foreach (var logicalNode in _modelRoot.LogicalNodes) ConvertNode(logicalNode);
            var buffer = new MemoryStream();
            using (var writer = new ContentWriter(buffer, true))
            {
                writer.Write(_content);
            }

            Content = buffer.ToArray();
        }

        private void ConvertNode(Node logicalNode)
        {
            if (_content.Node.Count != logicalNode.LogicalIndex)
                throw new InvalidDataException("Index mismatch");
            var nodeData = new NodeData
            {
                Name = _content.AddString(logicalNode.Name),
                ParentNode = logicalNode.VisualParent == null ? -1 : logicalNode.VisualParent.LogicalIndex,
                MeshIndex = logicalNode.Mesh == null ? -1 : logicalNode.Mesh.LogicalIndex,
                LocalTransform = logicalNode.LocalMatrix,
                WorldTransform = logicalNode.WorldMatrix
            };
            _content.Node.Add(nodeData);
        }

        private void ConvertMesh(Mesh mesh, Skin skin)
        {
            var meshData = new MeshData(new IndexRange(_content.Primitive.Count, mesh.Primitives.Count));
            meshData.Name = _content.AddString(mesh.Name);
            foreach (var primitive in mesh.Primitives)
            {
                var primitiveData = new PrimitiveData();
                primitiveData.IndexBuffer = 1;
                primitiveData.IndexBufferOffset = (uint) _indexBuffer.Position;
                primitiveData.VertexBufferView = _content.BufferViews.Count;
                _content.BufferViews.Add(new VertexBufferViewData(0, (uint) _vertexBuffer.Position,
                    new IndexRange(0, 1)));
                List<int> indices;
                switch (primitive.DrawPrimitiveType)
                {
                    case PrimitiveType.POINTS:
                        indices = primitive.GetPointIndices().ToList();
                        primitiveData.PrimitiveTopology = PrimitiveTopology.PointList;
                        break;
                    case PrimitiveType.LINES:
                        indices = GetLineIndices(primitive).ToList();
                        primitiveData.PrimitiveTopology = PrimitiveTopology.LineList;
                        break;
                    case PrimitiveType.LINE_LOOP:
                        indices = GetLineIndices(primitive).ToList();
                        primitiveData.PrimitiveTopology = PrimitiveTopology.LineList;
                        break;
                    case PrimitiveType.LINE_STRIP:
                        indices = GetLineIndices(primitive).ToList();
                        primitiveData.PrimitiveTopology = PrimitiveTopology.LineList;
                        break;
                    case PrimitiveType.TRIANGLES:
                        indices = GetTriangleIndices(primitive).ToList();
                        primitiveData.PrimitiveTopology = PrimitiveTopology.TriangleList;
                        break;
                    case PrimitiveType.TRIANGLE_STRIP:
                        indices = GetTriangleIndices(primitive).ToList();
                        primitiveData.PrimitiveTopology = PrimitiveTopology.TriangleList;
                        break;
                    case PrimitiveType.TRIANGLE_FAN:
                        indices = GetTriangleIndices(primitive).ToList();
                        primitiveData.PrimitiveTopology = PrimitiveTopology.TriangleList;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var vertexAccessors = primitive.VertexAccessors;
                var position = vertexAccessors["POSITION"].AsVector3Array();
                var newIndices = new Dictionary<int, int>();
                primitiveData.IndexBufferFormat =
                    indices.Count <= ushort.MaxValue ? IndexFormat.UInt16 : IndexFormat.UInt32;
                foreach (var index in indices)
                {
                    int v;
                    if (!newIndices.TryGetValue(index, out v))
                    {
                        v = newIndices.Count;
                        newIndices.Add(index, v);
                        var vector3 = position[index];
                        _vertexWriter.Write(vector3.X);
                        _vertexWriter.Write(vector3.Y);
                        _vertexWriter.Write(vector3.Z);
                    }

                    if (primitiveData.IndexBufferFormat == IndexFormat.UInt16)
                        _indexWriter.Write((ushort) v);
                    else
                        _indexWriter.Write((uint) v);
                }

                primitiveData.IndexCount = (uint) indices.Count;

                //foreach (var vertexAccessor in vertexAccessors)
                //{
                //    var key = vertexAccessor.Key;
                //    var accessor = vertexAccessor.Value;
                //    if (k)
                //}
                _content.Primitive.Add(primitiveData);
            }

            _content.Mesh.Add(meshData);
        }

        private IEnumerable<int> GetTriangleIndices(MeshPrimitive primitive)
        {
            foreach (var valueTuple in primitive.GetTriangleIndices())
            {
                yield return valueTuple.A;
                yield return valueTuple.B;
                yield return valueTuple.C;
            }
        }

        private IEnumerable<int> GetLineIndices(MeshPrimitive primitive)
        {
            foreach (var valueTuple in primitive.GetLineIndices())
            {
                yield return valueTuple.A;
                yield return valueTuple.B;
            }
        }
    }
}