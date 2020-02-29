using System;
using System.Collections.Generic;
using System.IO;
using Veldrid.PBR.BinaryData;

namespace Veldrid.PBR
{
    public class ContentToWrite
    {
        private readonly List<ArraySegment<byte>> _binaryBlobs = new List<ArraySegment<byte>>();
        private readonly Dictionary<string, int> _stringMap = new Dictionary<string, int>();
        private readonly List<string> _strings = new List<string>();

        public IReadOnlyList<ArraySegment<byte>> BinaryBlobs => _binaryBlobs;

        public IList<BufferData> Buffers { get; } = new List<BufferData>();
        public IReadOnlyList<string> Strings => _strings;
        public IList<TextureData> Textures { get; } = new List<TextureData>();
        public IList<SamplerData> Samplers { get; } = new List<SamplerData>();
        public IList<UnlitMaterialData> UnlitMaterials { get; } = new List<UnlitMaterialData>();
        public IList<MetallicRoughnessMaterialData> MetallicRoughnessMaterials { get; } = new List<MetallicRoughnessMaterialData>();
        public IList<SpecularGlossinessMaterialData> SpecularGlossinessMaterials { get; } = new List<SpecularGlossinessMaterialData>();
        public IList<MaterialReference> MaterialBindings { get; } = new List<MaterialReference>();
        public IList<VertexElementData> VertexElements { get; } = new List<VertexElementData>();
        public IList<VertexBufferViewData> BufferViews { get; } = new List<VertexBufferViewData>();
        public IList<PrimitiveData> Primitive { get; } = new List<PrimitiveData>();
        public IList<MeshData> Mesh { get; } = new List<MeshData>();
        public IList<NodeData> Node { get; } = new List<NodeData>();

        public int AddBlob(ArraySegment<byte> segment)
        {
            var index = BinaryBlobs.Count;
            _binaryBlobs.Add(segment);
            return index;
        }

        public int AddBlob(byte[] array)
        {
            var index = BinaryBlobs.Count;
            _binaryBlobs.Add(new ArraySegment<byte>(array));
            return index;
        }

        public int AddBlob(MemoryStream buffer)
        {
            var index = BinaryBlobs.Count;
            _binaryBlobs.Add(new ArraySegment<byte>(buffer.ToArray()));
            return index;
        }

        public int AddString(string str)
        {
            str = str ?? string.Empty;
            if (_stringMap.TryGetValue(str, out var index))
                return index;
            index = _strings.Count;
            _stringMap.Add(str, index);
            _strings.Add(str);
            return index;
        }
    }
}