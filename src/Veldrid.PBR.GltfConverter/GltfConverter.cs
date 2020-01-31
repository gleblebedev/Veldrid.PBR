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
            Content = new PbrContent();
            _indexBuffer = new MemoryStream();
            _indexWriter = new BinaryWriter(_indexBuffer);
            _vertexBuffer = new MemoryStream();
            _vertexWriter = new BinaryWriter(_vertexBuffer);
        }
        public static PbrContent ReadGtlf(string fileName)
        {
            var modelRoot = SharpGLTF.Schema2.ModelRoot.Load(fileName, new ReadSettings() {Validation = ValidationMode.Skip});
            var converter = new GltfConverter(modelRoot);
            converter.Convert();
            return converter.Content;
        }

        public PbrContent Content { get;}

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
            Content.Buffers.Add(new BufferData(){});
        }

        public List<GtlfPrimitiveData> _primitives = new List<GtlfPrimitiveData>();
        public MemoryStream _indexBuffer;
        public BinaryWriter _indexWriter;
        public MemoryStream _vertexBuffer;
        private BinaryWriter _vertexWriter;

        private void ConvertMesh(Mesh mesh, Skin skin)
        {
            foreach (var primitive in mesh.Primitives)
            {
                List<int> indices;
                var primitiveData = new GtlfPrimitiveData();
                switch (primitive.DrawPrimitiveType)
                {
                    case PrimitiveType.POINTS:
                        indices = primitive.GetPointIndices().ToList();
                        primitiveData.Topology = PrimitiveTopology.PointList;
                        break;
                    case PrimitiveType.LINES:
                        indices = GetLineIndices(primitive).ToList();
                        primitiveData.Topology = PrimitiveTopology.LineList;
                        break;
                    case PrimitiveType.LINE_LOOP:
                        indices = GetLineIndices(primitive).ToList();
                        primitiveData.Topology = PrimitiveTopology.LineList;
                        break;
                    case PrimitiveType.LINE_STRIP:
                        indices = GetLineIndices(primitive).ToList();
                        primitiveData.Topology = PrimitiveTopology.LineList;
                        break;
                    case PrimitiveType.TRIANGLES:
                        indices = GetTriangleIndices(primitive).ToList();
                        primitiveData.Topology = PrimitiveTopology.TriangleList;
                        break;
                    case PrimitiveType.TRIANGLE_STRIP:
                        indices = GetTriangleIndices(primitive).ToList();
                        primitiveData.Topology = PrimitiveTopology.TriangleList;
                        break;
                    case PrimitiveType.TRIANGLE_FAN:
                        indices = GetTriangleIndices(primitive).ToList();
                        primitiveData.Topology = PrimitiveTopology.TriangleList;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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