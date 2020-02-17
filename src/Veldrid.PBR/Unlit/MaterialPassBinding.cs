namespace Veldrid.PBR.Unlit
{
    public class MaterialPassBinding: IPassBinding
    {
        private readonly Pipeline _pipeline;
        private readonly uint _indexCount;
        private readonly ResourceSet _resourceSet;
        private uint _modelUniformOffset;


        public MaterialPassBinding(Pipeline pipeline, uint indexCount, ResourceSet resourceSet, uint modelUniformOffset)
        {
            _pipeline = pipeline;
            _indexCount = indexCount;
            _resourceSet = resourceSet;
            _modelUniformOffset = modelUniformOffset;
        }

        public void Draw(CommandList commandList)
        {
            commandList.SetPipeline(_pipeline);
            commandList.SetGraphicsResourceSet(0, _resourceSet, 1, ref _modelUniformOffset);
            commandList.DrawIndexed(_indexCount, 1, 0, 0, 0);

        }
    }
}