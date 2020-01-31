using System.Runtime.InteropServices;

namespace Veldrid.PBR
{
    public class MeshData
    {
        public MeshData()
        {
        }

        public IndexRange Primitives;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PrimitiveData
    {
        public int Pipeline;
        public int IndexBuffer;
        public IndexFormat IndexBufferFormat;
        public uint IndexBufferOffset;
        public uint IndexCount;
        public int VertexBuffer;
        public uint VertexBufferOffset;
    }
}