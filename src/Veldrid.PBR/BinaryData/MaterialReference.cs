using System.Runtime.InteropServices;

namespace Veldrid.PBR.BinaryData
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MaterialReference
    {
        public IdRef Material;
        public MaterialType MaterialType;

        public MaterialReference(MaterialType materialType, int materialIndex)
        {
            Material = new IdRef(materialIndex);
            MaterialType = materialType;
        }
    }
}