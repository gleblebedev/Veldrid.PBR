namespace Veldrid.PBR.BinaryData
{
    public struct BufferData
    {
        public BufferData(int index, BufferDescription description)
        {
            Usage = description.Usage;
            StructureByteStride = description.StructureByteStride;
            RawBuffer = description.RawBuffer;
            BlobIndex = index;
        }

        public BufferData(int index, BufferUsage usage, uint structureByteStride, bool rawBuffer)
        {
            Usage = usage;
            StructureByteStride = structureByteStride;
            RawBuffer = rawBuffer;
            BlobIndex = index;
        }

        public BufferData(int index, BufferUsage usage)
        {
            Usage = usage;
            StructureByteStride = 0;
            RawBuffer = false;
            BlobIndex = index;
        }

        /// <summary>
        ///     Binary blob index.
        /// </summary>
        public int BlobIndex;

        /// <summary>
        ///     Indicates how the <see cref="DeviceBuffer" /> will be used.
        /// </summary>
        public BufferUsage Usage;

        /// <summary>
        ///     For structured buffers, this value indicates the size in bytes of a single structure element, and must be non-zero.
        ///     For all other buffer types, this value must be zero.
        /// </summary>
        public uint StructureByteStride;

        /// <summary>
        ///     Indicates that this is a raw buffer. This should be combined with
        ///     <see cref="BufferUsage.StructuredBufferReadWrite" />. This affects how the buffer is bound in the D3D11 backend.
        /// </summary>
        public bool RawBuffer;
    }
}