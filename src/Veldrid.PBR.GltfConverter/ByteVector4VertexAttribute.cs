using System;
using System.IO;
using Veldrid.PBR.Numerics;

namespace Veldrid.PBR
{
    internal class ByteVector4VertexAttribute : AbstractVertexAttribute
    {
        private readonly ByteVector4 _value;
        private readonly bool _normalized;

        public ByteVector4VertexAttribute(string key, ByteVector4 value, bool normalized) : base(key)
        {
            _value = value;
            _normalized = normalized;
        }

        public override VertexElementFormat VertexElementFormat => _normalized?VertexElementFormat.Byte4_Norm: VertexElementFormat.Byte4;
        public override int Count { get; }
        static readonly Random rnd = new Random(0);
        public override void Write(BinaryWriter vertexWriter, int index)
        {
            vertexWriter.Write(_value.X);
            vertexWriter.Write(_value.Y);
            vertexWriter.Write(_value.Z);
            vertexWriter.Write(_value.W);
        }
    }
}