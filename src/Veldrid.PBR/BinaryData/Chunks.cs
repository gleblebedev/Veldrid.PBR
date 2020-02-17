using System;

namespace Veldrid.PBR.BinaryData
{
    public struct Chunks
    {
        public Chunk<IndexRange> BinaryBlobs;
        public Chunk<IndexRange> Strings;
        public Chunk<BufferData> Buffers;
        public Chunk<SamplerData> Samplers;
        public Chunk<TextureData> Textures;
        public Chunk<VertexElementData> VertexElements;
        public Chunk<VertexBufferViewData> BufferViews;
        public Chunk<UnlitMaterialData> UnlitMaterials;
        public Chunk<MaterialReference> MaterialBindings;
        public Chunk<PrimitiveData> Primitives;
        public Chunk<MeshData> Meshes;
        public Chunk<NodeData> Nodes;

    }
}