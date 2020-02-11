namespace Veldrid.PBR.BinaryData
{
    public struct MeshData
    {
        public MeshData(IndexRange primitives)
        {
            Primitives = primitives;
            Name = -1;
        }

        public IndexRange Primitives;
        public int Name;
    }
}