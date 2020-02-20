namespace Veldrid.PBR
{
    public class MaterialPassBinding : IPassBinding
    {
        private readonly Pipeline _pipeline;
        private readonly uint _indexCount;
        private ResourceSetAndOffsets _slot0;
        private ResourceSetAndOffsets _slot1;

        public MaterialPassBinding(Pipeline pipeline, uint indexCount, in ResourceSetAndOffsets slot0,
            in ResourceSetAndOffsets slot1)
        {
            _pipeline = pipeline;
            _indexCount = indexCount;
            _slot0 = slot0;
            _slot1 = slot1;
        }

        public void Draw(CommandList commandList)
        {
            commandList.SetPipeline(_pipeline);
            commandList.SetGraphicsResourceSet(0, ref _slot0);
            commandList.SetGraphicsResourceSet(1, ref _slot1);
            commandList.DrawIndexed(_indexCount, 1, 0, 0, 0);
        }
    }
}