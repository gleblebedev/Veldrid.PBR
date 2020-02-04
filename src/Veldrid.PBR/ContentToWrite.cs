using System.Collections.Generic;

namespace Veldrid.PBR
{
    public class ContentToWrite
    {
        public IList<byte[]> BufferData { get; } = new List<byte[]>();
        public IList<BufferDescription> BufferDescription { get; } = new List<BufferDescription>();
        public IList<PrimitiveData> Primitive { get; } = new List<PrimitiveData>();
        public IList<MeshData> Mesh { get; } = new List<MeshData>();
        public IList<NodeData> Node { get; } = new List<NodeData>();
    }
}