namespace Veldrid.PBR
{
    public struct Chunks
    {
        public Chunk<IndexRange> BinaryBlobs;
        public Chunk<IndexRange> Strings;
        public Chunk<BufferData> Buffers;
        public Chunk<TextureData> Textures;
        public Chunk<VertexElementData> VertexElements;
        public Chunk<VertexBufferViewData> BufferViews;
        public Chunk<MaterialData> Materials;
        public Chunk<PrimitiveData> Primitives;
        public Chunk<MeshData> Meshes;
        public Chunk<NodeData> Nodes;
    }
}