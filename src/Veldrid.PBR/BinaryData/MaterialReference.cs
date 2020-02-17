using System.Runtime.InteropServices;

namespace Veldrid.PBR.BinaryData
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MaterialReference
    {
        public MaterialType MaterialType;
        public int Material;

        public MaterialReference(MaterialType materialType, int materialIndex)
        {
            MaterialType = materialType;
            Material = materialIndex;
        }
    }
}