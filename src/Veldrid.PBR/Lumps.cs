namespace Veldrid.PBR
{
    public struct Lumps
    {
        public Lump<IndexRange> BinaryBlobs;
        public Lump<IndexRange> Strings;
        public Lump<BufferData> Buffers;
        public Lump<TextureData> Textures;
        public Lump<VertexElementData> VertexElements;
        public Lump<VertexBufferViewData> BufferViews;
        public Lump<PrimitiveData> Primitives;
        public Lump<MeshData> Meshes;
        public Lump<NodeData> Nodes;
    }
}