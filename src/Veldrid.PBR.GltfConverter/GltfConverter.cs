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
        private readonly ModelRoot _modelRoot;

        internal GltfConverter(ModelRoot modelRoot)
        {
            _modelRoot = modelRoot;
            _content = new ContentToWrite();
            _indexBuffer = new MemoryStream();
            _indexWriter = new BinaryWriter(_indexBuffer);
            _vertexBuffer = new MemoryStream();
            _vertexWriter = new BinaryWriter(_vertexBuffer);
        }
        public static byte[] ReadGtlf(string fileName)
        {
            var modelRoot = SharpGLTF.Schema2.ModelRoot.Load(fileName, new ReadSettings() {Validation = ValidationMode.Skip});
            var converter = new GltfConverter(modelRoot);
            converter.Convert();
            return converter.Content;
        }

        public byte[] Content { get; private set; } 

        private void Convert()
        {
            var skinPerMesh = new Skin[_modelRoot.LogicalMeshes.Count];
            foreach (var node in _modelRoot.LogicalNodes)
            {
                if (node.Skin != null && node.Mesh != null)
                {
                    skinPerMesh[node.Mesh.LogicalIndex] = node.Skin;
                }
            }
            foreach (var mesh in _modelRoot.LogicalMeshes)
            {
                ConvertMesh(mesh, skinPerMesh[mesh.LogicalIndex]);
            }
            _content.BufferDescription.Add(new BufferDescription(0, BufferUsage.VertexBuffer));
            _content.BufferDescription.Add(new BufferDescription(0, BufferUsage.IndexBuffer));
            _content.BufferData.Add(_vertexBuffer.ToArray());
            _content.BufferData.Add(_indexBuffer.ToArray());
            var buffer = new MemoryStream();
            using (var writer = new ContentWriter(buffer, true))
            {
                writer.Write(_content);
            }
            Content = buffer.ToArray();
        }

        public MemoryStream _indexBuffer;
        public BinaryWriter _indexWriter;
        public MemoryStream _vertexBuffer;
        private BinaryWriter _vertexWriter;
        private ContentToWrite _content;

        private void ConvertMesh(Mesh mesh, Skin skin)
        {
            var meshData = new MeshData();
            meshData.Primitives = new IndexRange(_content.Primitive.Count, mesh.Primitives.Count);
            foreach (var primitive in mesh.Primitives)
            {
                var primitiveData = new PrimitiveData();
                primitiveData.IndexBuffer = 1;
                primitiveData.IndexBufferOffset = (uint)_indexBuffer.Position;
                primitiveData.VertexBuffer = 0;
                primitiveData.VertexBufferOffset = (uint)_vertexBuffer.Position;
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
                    (indices.Count <= UInt16.MaxValue) ? IndexFormat.UInt16 : IndexFormat.UInt32;
                foreach (var index in indices)
                {
                    int v;
                    if (!newIndices.TryGetValue(index, out v))
                    {
                        v = newIndices.Count;
                        newIndices.Add(index,v);
                        var vector3 = position[index];
                        _vertexWriter.Write(vector3.X);
                        _vertexWriter.Write(vector3.Y);
                        _vertexWriter.Write(vector3.Z);
                    }
                    if (primitiveData.IndexBufferFormat == IndexFormat.UInt16)
                        _indexWriter.Write((ushort)v);
                    else
                        _indexWriter.Write((uint)v);
                }
                primitiveData.IndexCount = (uint)indices.Count;

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