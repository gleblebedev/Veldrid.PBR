using System;
using System.Runtime.CompilerServices;
using Veldrid.PBR.BinaryData;

namespace Veldrid.PBR
{
    internal static class ExtensionMethods
    {
        public static Span<T> Slice<T>(this in Span<T> span, IndexRange range)
        {
            return span.Slice(range.StartIndex, range.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetGraphicsResourceSet(this CommandList commandList, uint slot,
            ref ResourceSetAndOffsets resourceSet)
        {
            if (resourceSet.ResourceSet != null)
                commandList.SetGraphicsResourceSet(slot, resourceSet.ResourceSet, resourceSet.OffsetCount,
                    ref resourceSet.Offset0);
        }

        public static int GetNumComponents(this VertexElementFormat format)
        {
            switch (format)
            {
                case VertexElementFormat.Float1:
                case VertexElementFormat.UInt1:
                case VertexElementFormat.Int1:
                case VertexElementFormat.Half1:
                    return 1;
                case VertexElementFormat.Float2:
                case VertexElementFormat.Byte2_Norm:
                case VertexElementFormat.Byte2:
                case VertexElementFormat.SByte2_Norm:
                case VertexElementFormat.SByte2:
                case VertexElementFormat.UShort2_Norm:
                case VertexElementFormat.UShort2:
                case VertexElementFormat.Short2_Norm:
                case VertexElementFormat.Short2:
                case VertexElementFormat.UInt2:
                case VertexElementFormat.Int2:
                case VertexElementFormat.Half2:
                    return 2;
                case VertexElementFormat.Float3:
                case VertexElementFormat.UInt3:
                case VertexElementFormat.Int3:
                    return 3;
                case VertexElementFormat.Float4:
                case VertexElementFormat.Byte4_Norm:
                case VertexElementFormat.Byte4:
                case VertexElementFormat.SByte4_Norm:
                case VertexElementFormat.SByte4:
                case VertexElementFormat.UShort4_Norm:
                case VertexElementFormat.UShort4:
                case VertexElementFormat.Short4_Norm:
                case VertexElementFormat.Short4:
                case VertexElementFormat.UInt4:
                case VertexElementFormat.Int4:
                case VertexElementFormat.Half4:
                    return 4;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }
    }
}