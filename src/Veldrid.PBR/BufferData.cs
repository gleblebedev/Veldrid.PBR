namespace Veldrid.PBR
{
    public struct BufferData
    {
        public BufferData(BufferDescription description, int index)
        {
            Description = description;
            BlobIndex = index;
        }

        public BufferDescription Description;
        public int BlobIndex;
    }
}