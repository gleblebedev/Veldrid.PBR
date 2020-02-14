﻿using System.Runtime.InteropServices;

namespace Veldrid.PBR.BinaryData
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PrimitiveData
    {
        /// <summary>
        ///     The <see cref="PrimitiveTopology" /> to use, which controls how a series of input vertices is interpreted by the
        ///     <see cref="Pipeline" />.
        /// </summary>
        public PrimitiveTopology PrimitiveTopology;

        public int IndexBuffer;
        public IndexFormat IndexBufferFormat;
        public uint IndexBufferOffset;
        public uint IndexCount;
        public int VertexBufferView;
        public MaterialType MaterialType;
        public int Material;
    }
}