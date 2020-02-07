namespace Veldrid.PBR
{
    public struct TextureData
    {
        public int BlobIndex;

        public TextureData(int index)
        {
            BlobIndex = index;
            Name = -1;
        }

        public int Name { get; set; }
    }
}