using System;
using System.IO;
using SharpGLTF.Schema2;

namespace Veldrid.PBR
{
    internal abstract class AbstractVertexAttribute
    {
        public abstract VertexElementFormat VertexElementFormat { get; }

        public string Key { get; }

        public int Priority { get; }

        public AbstractVertexAttribute(string key)
        {
            Key = key;
            var subKey = key;
            Priority = 0;
            if (subKey.StartsWith(GltfConverter.TargetPrefix))
            {
                subKey = subKey.Substring(GltfConverter.TargetPrefix.Length);
                Priority += 1;
            }

            if (subKey.StartsWith("POSITION"))
            {
            }
            else if (subKey.StartsWith("NORMAL"))
            {
                Priority += 2;
            }
            else if (subKey.StartsWith("TANGENT"))
            {
                Priority += 4;
            }
            else if (subKey.StartsWith("TEXCOORD"))
            {
                Priority += 6;
            }
            else if (subKey.StartsWith("COLOR"))
            {
                Priority += 8;
            }
            else if (subKey.StartsWith("JOINTS"))
            {
                Priority += 10;
            }
            else if (subKey.StartsWith("WEIGHTS"))
            {
                Priority += 12;
            }
            else
            {
                Priority += 14;
            }
        }

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
                    throw new ArgumentOutOfRangeException(accessor.Encoding.ToString());
            }
        }

        private static AbstractVertexAttribute CreateVec2VertexAttribute(string key, Accessor accessor)
        {
            switch (accessor.Encoding)
            {
                case EncodingType.FLOAT:
                    return new Float2VertexAttribute(key, accessor);
                case EncodingType.UNSIGNED_SHORT:
                    return new UShort2VertexAttribute(key, accessor);
                default:
                    return new Float2VertexAttribute(key, accessor);
            }
        }

        private static AbstractVertexAttribute CreateVec3VertexAttribute(string key, Accessor accessor)
        {
            switch (accessor.Encoding)
            {
                case EncodingType.FLOAT:
                    return new Float3VertexAttribute(key, accessor);
                default:
                    return new Float3VertexAttribute(key, accessor);
            }
        }

        private static AbstractVertexAttribute CreateVec4VertexAttribute(string key, Accessor accessor)
        {
            switch (accessor.Encoding)
            {
                case EncodingType.BYTE:
                    if (accessor.Normalized)
                        return new Byte4_NormVertexAttribute(key, accessor);
                    else
                        return new Byte4VertexAttribute(key, accessor);
                case EncodingType.FLOAT:
                    return new Float4VertexAttribute(key, accessor);
                default:
                    return new Float4VertexAttribute(key, accessor);
            }
        }

        public virtual void Write(BinaryWriter vertexWriter, int index)
        {
            throw new NotImplementedException();
        }
    }
}