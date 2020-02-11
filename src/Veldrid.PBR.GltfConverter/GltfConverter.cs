using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using SharpGLTF.Schema2;
using SharpGLTF.Validation;

namespace Veldrid.PBR
{
    public class GltfConverter
    {
        public const string TargetPrefix = "TARGET_";
        public MemoryStream _indexBuffer;
        public BinaryWriter _indexWriter;
        public MemoryStream _vertexBuffer;
        private readonly ModelRoot _modelRoot;
        private readonly BinaryWriter _vertexWriter;
        private readonly ContentToWrite _content;
        private Dictionary<ComparableList<VertexElementData>, IndexRange> _elements = new Dictionary<ComparableList<VertexElementData>, IndexRange>();

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

                var attributes = GetVertexAttributes(primitive);

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
                        foreach (var attribute in attributes) attribute.Write(_vertexWriter, index);
                    }

                    if (primitiveData.IndexBufferFormat == IndexFormat.UInt16)
                        _indexWriter.Write((ushort) v);
                    else
                        _indexWriter.Write((uint) v);
                }

                primitiveData.IndexCount = (uint) indices.Count;

                _content.Primitive.Add(primitiveData);
            }

            _content.Mesh.Add(meshData);
        }

        private List<AbstractVertexAttribute> GetVertexAttributes(MeshPrimitive primitive)
        {
            var vertexAccessors = primitive.VertexAccessors;
            var attributes = new List<AbstractVertexAttribute>(vertexAccessors.Count);
            AbstractVertexAttribute positions = null;
            AbstractVertexAttribute normals = null;
            foreach (var vertexAccessor in vertexAccessors)
            {
                var key = vertexAccessor.Key;
                var accessor = vertexAccessor.Value;
                var attribute = AbstractVertexAttribute.Create(key, accessor);
                attributes.Add(attribute);
                switch (key)
                {
                    case "POSITION":
                        positions = attribute;
                        break;
                    case "NORMAL":
                        normals = attribute;
                        break;
                }
            }

            if (normals == null)
            {
                normals = GenerateNormals((Float3VertexAttribute)positions, primitive);
                attributes.Add(normals);
            }
            //else
            //{
            //    var normals1 = GenerateNormals((Float3VertexAttribute)positions, primitive);
            //    var normalsVec3 = ((Float3VertexAttribute)normals);
            //    for (var index = 0; index < normals1.Values.Length; index++)
            //    {
            //        var p = Vector3.Dot(normals1.Values[index], normalsVec3.Values[index]);
            //        if (p < 0.0f)
            //            throw new Exception("InvalidNormalDirection");
            //    }
            //}

            for (int morphTargetIndex = 0; morphTargetIndex < primitive.MorphTargetsCount; ++morphTargetIndex)
            {
                foreach (var morphTargetAccessor in primitive.GetMorphTargetAccessors(morphTargetIndex))
                {
                    var key = TargetPrefix+morphTargetAccessor.Key+"_"+morphTargetIndex;
                    var accessor = morphTargetAccessor.Value;
                    var attribute = AbstractVertexAttribute.Create(key, accessor);
                    attributes.Add(attribute);
                }
            }
            attributes.Sort(new VertexAttributeComparer());
            var elements = new ComparableList<VertexElementData>(attributes.Select(_=> new VertexElementData(_content.AddString(_.Key), _.VertexElementFormat, VertexElementSemantic.TextureCoordinate)));


            if (!_elements.TryGetValue(elements, out var range))
            {
                range = new IndexRange(_content.VertexElements.Count, elements.Count);
                foreach (var element in elements)
                {
                    _content.VertexElements.Add(element);
                }
            }


            var vertexBufferViewData = new VertexBufferViewData(0, (uint) _vertexBuffer.Position, range);

            _content.BufferViews.Add(vertexBufferViewData);

            return attributes;
        }

        private Vector3ArrayVertexAttribute GenerateNormals(Float3VertexAttribute positions, MeshPrimitive primitive)
        {
            var normals = new Vector3[positions.Values.Count];
            if (primitive.DrawPrimitiveType == PrimitiveType.TRIANGLES
                || primitive.DrawPrimitiveType == PrimitiveType.TRIANGLE_FAN
                || primitive.DrawPrimitiveType == PrimitiveType.TRIANGLE_STRIP)
            {
                foreach ((int A, int B, int C) in primitive.GetTriangleIndices())
                {
                    var ab = positions.Values[B] - positions.Values[A];
                    var ac = positions.Values[C] - positions.Values[A];
                    var n = Vector3.Cross(ab, ac);
                    normals[A] += n;
                    normals[B] += n;
                    normals[C] += n;
                }
            }

            for (var index = 0; index < normals.Length; index++)
            {
                var normal = normals[index];
                if (normal != Vector3.Zero)
                    normals[index] = Vector3.Normalize(normal);
                else
                    normals[index] = Vector3.UnitY;
            }

            return new Vector3ArrayVertexAttribute("NORMAL", normals);
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