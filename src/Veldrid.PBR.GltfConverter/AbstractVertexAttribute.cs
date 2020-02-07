using System;
using System.IO;
using SharpGLTF.Schema2;

namespace Veldrid.PBR
{
    internal abstract class AbstractVertexAttribute
    {
        public abstract VertexElementFormat VertexElementFormat { get; }

        public static AbstractVertexAttribute Create(string key, Accessor accessor)
        {
            switch (accessor.Dimensions)
            {
                case DimensionType.VEC2:
                    return CreateVec2VertexAttribute(key, accessor);
                case DimensionType.VEC3:
                    return CreateVec3VertexAttribute(key, accessor);
                case DimensionType.VEC4:
                    return CreateVec4VertexAttribute(key, accessor);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static AbstractVertexAttribute CreateVec2VertexAttribute(string key, Accessor accessor)
        {
            switch (accessor.Encoding)
            {
                case EncodingType.FLOAT:
                    return new Float2VertexAttribute(key, accessor);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static AbstractVertexAttribute CreateVec3VertexAttribute(string key, Accessor accessor)
        {
            switch (accessor.Encoding)
            {
                case EncodingType.FLOAT:
                    return new Float3VertexAttribute(key, accessor);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static AbstractVertexAttribute CreateVec4VertexAttribute(string key, Accessor accessor)
        {
            switch (accessor.Encoding)
            {
                case EncodingType.FLOAT:
                    return new Float4VertexAttribute(key, accessor);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public virtual void Write(BinaryWriter vertexWriter, int index)
        {
            throw new NotImplementedException();
        }
    }
}