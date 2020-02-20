using System.Runtime.InteropServices;

namespace Veldrid.PBR
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ResourceSetAndOffsets
    {
        public ResourceSetAndOffsets(ResourceSet resourceSet)
        {
            ResourceSet = resourceSet;
            OffsetCount = 0;
            Offset0 = 0;
            Offset1 = 0;
            Offset2 = 0;
            Offset3 = 0;
        }

        public ResourceSetAndOffsets(ResourceSet resourceSet, uint offset)
        {
            ResourceSet = resourceSet;
            OffsetCount = 1;
            Offset0 = offset;
            Offset1 = 0;
            Offset2 = 0;
            Offset3 = 0;
        }

        public ResourceSetAndOffsets(ResourceSet resourceSet, uint offset0, uint offset1)
        {
            ResourceSet = resourceSet;
            OffsetCount = 1;
            Offset0 = offset0;
            Offset1 = offset1;
            Offset2 = 0;
            Offset3 = 0;
        }

        public ResourceSetAndOffsets(ResourceSet resourceSet, uint offset0, uint offset1, uint offset2)
        {
            ResourceSet = resourceSet;
            OffsetCount = 1;
            Offset0 = offset0;
            Offset1 = offset1;
            Offset2 = offset2;
            Offset3 = 0;
        }

        public ResourceSetAndOffsets(ResourceSet resourceSet, uint offset0, uint offset1, uint offset2, uint offset3)
        {
            ResourceSet = resourceSet;
            OffsetCount = 1;
            Offset0 = offset0;
            Offset1 = offset1;
            Offset2 = offset2;
            Offset3 = offset3;
        }

        public ResourceSet ResourceSet;
        public uint OffsetCount;
        public uint Offset0;
        public uint Offset1;
        public uint Offset2;
        public uint Offset3;
    }
}