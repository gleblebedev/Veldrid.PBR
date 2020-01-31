using System.Collections.Generic;

namespace Veldrid.PBR
{
    public class PbrContent
    {
        public IList<MeshData> Meshes { get; } = new List<MeshData>();
        public IList<BufferData> Buffers { get; } = new List<BufferData>();
        public IList<PrimitiveData> Primitives { get; } = new List<PrimitiveData>();
        public IList<PipelineData> Pipelines { get; } = new List<PipelineData>();
    }
}