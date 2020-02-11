using System.Runtime.InteropServices;

namespace Veldrid.PBR.BinaryData
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexBufferViewData
    {
        public VertexBufferViewData(int bufferIndex, uint offset, IndexRange elements)
        {
            Buffer = bufferIndex;
            Offset = offset;
            Elements = elements;
        }

        public int Buffer;
        public uint Offset;
        public IndexRange Elements;
    }
}