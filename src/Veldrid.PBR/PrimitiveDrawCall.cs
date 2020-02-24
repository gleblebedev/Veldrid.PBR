namespace Veldrid.PBR
{
    public struct PrimitiveDrawCall
    {
        public PrimitiveTopology Topology;
        public uint IndexCount;
        public DeviceBuffer VertexBuffer;
        public uint VertexBufferOffset;
        public DeviceBuffer IndexBuffer;
        public IndexFormat IndexBufferFormat;
        public uint IndexBufferOffset;
        public uint ModelUniformOffset;
        public VertexLayoutDescription VertexLayoutDescription;
    }
}