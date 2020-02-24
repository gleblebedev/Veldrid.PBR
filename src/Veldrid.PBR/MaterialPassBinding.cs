namespace Veldrid.PBR
{
    public class MaterialPassBinding : IPassBinding
    {
        private readonly Pipeline _pipeline;
        private readonly uint _indexCount;
        private ResourceSetAndOffsets _slot0;
        private ResourceSetAndOffsets _slot1;
        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _indexBuffer;
        private uint _vertexBufferOffset;
        private uint _indexBufferOffset;
        private IndexFormat _indexBufferFormat;

        public MaterialPassBinding(Pipeline pipeline, ref PrimitiveDrawCall drawCall, in ResourceSetAndOffsets slot0,
            in ResourceSetAndOffsets slot1)
        {
            _pipeline = pipeline;
            _indexCount = drawCall.IndexCount;
            _vertexBuffer = drawCall.VertexBuffer;
            _vertexBufferOffset = drawCall.VertexBufferOffset;
            _indexBuffer = drawCall.IndexBuffer;
            _indexBufferFormat = drawCall.IndexBufferFormat;
            _indexBufferOffset = drawCall.IndexBufferOffset;
            _slot0 = slot0;
            _slot1 = slot1;
        }

        public void Draw(CommandList commandList)
        {
            commandList.SetVertexBuffer(0, _vertexBuffer, _vertexBufferOffset);
            commandList.SetIndexBuffer(_indexBuffer, _indexBufferFormat, _indexBufferOffset);
            commandList.SetPipeline(_pipeline);
            commandList.SetGraphicsResourceSet(0, ref _slot0);
            commandList.SetGraphicsResourceSet(1, ref _slot1);
            commandList.DrawIndexed(_indexCount, 1, 0, 0, 0);
        }
    }
}