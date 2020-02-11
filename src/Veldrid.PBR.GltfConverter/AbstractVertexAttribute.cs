using System;
using System.IO;
using System.Runtime.CompilerServices;
using SharpDX.Win32;
using SharpGLTF.Schema2;

namespace Veldrid.PBR
{
    internal abstract class AbstractVertexAttribute
    {
        public abstract VertexElementFormat VertexElementFormat { get; }

        public string Key { get; }

        public int Priority { get; }

        public abstract int Count { get; }

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

        public override string ToString()
        {
            return Key;
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
                case EncodingType.BYTE:
                    if (accessor.Normalized)
                        return new SByte2_NormVertexAttribute(key, accessor);
                    else
                        return new SByte2VertexAttribute(key, accessor);
                case EncodingType.UNSIGNED_BYTE:
                    if (accessor.Normalized)
                        return new Byte2_NormVertexAttribute(key, accessor);
                    else
                        return new Byte2VertexAttribute(key, accessor);
                case EncodingType.SHORT:
                    if (accessor.Normalized)
                        return new Short2_NormVertexAttribute(key, accessor);
                    else
                        return new Short2VertexAttribute(key, accessor);
                case EncodingType.UNSIGNED_SHORT:
                    if (accessor.Normalized)
                        return new UShort2_NormVertexAttribute(key, accessor);
                    else
                        return new UShort2VertexAttribute(key, accessor);
                case EncodingType.FLOAT:
                    return new Float2VertexAttribute(key, accessor);
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
                        return new SByte4_NormVertexAttribute(key, accessor);
                    else
                        return new SByte4VertexAttribute(key, accessor);
                case EncodingType.UNSIGNED_BYTE:
                    if (accessor.Normalized)
                        return new Byte4_NormVertexAttribute(key, accessor);
                    else
                        return new Byte4VertexAttribute(key, accessor);
                case EncodingType.SHORT:
                    if (accessor.Normalized)
                        return new Short4_NormVertexAttribute(key, accessor);
                    else
                        return new Short4VertexAttribute(key, accessor);
                case EncodingType.UNSIGNED_SHORT:
                    if (accessor.Normalized)
                        return new UShort4_NormVertexAttribute(key, accessor);
                    else
                        return new UShort4VertexAttribute(key, accessor);
                case EncodingType.FLOAT:
                    return new Float4VertexAttribute(key, accessor);
                default:
                    return new Float4VertexAttribute(key, accessor);
            }
        }

        public abstract void Write(BinaryWriter vertexWriter, int index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static ushort FloatToUShort(float val)
        {
            return (ushort) val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static short FloatToShort(float val)
        {
            return (short)val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static byte FloatToByte(float val)
        {
            return (byte)val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static sbyte FloatToSByte(float val)
        {
            return (sbyte)val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static ushort FloatToNormUShort(float val)
        {
            return (ushort)NormalizeUnsigned(val, ushort.MaxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static short FloatToNormShort(float val)
        {
            return (short)NormalizeSigned(val, short.MaxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static byte FloatToNormByte(float val)
        {
            return (byte)NormalizeUnsigned(val, byte.MaxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static sbyte FloatToNormSByte(float val)
        {
            return (sbyte)NormalizeSigned(val, sbyte.MaxValue);
        }

        /// <summary>
        /// In OpenGL 4.2 and above, the conversion always happens by mapping the signed integer range [-MAX, MAX] to the float range [-1, 1].
        /// https://www.khronos.org/opengl/wiki/Normalized_Integer
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float NormalizeSigned(float val, float maxValue)
        {
            if (val <= -maxValue)
                return -1;
            if (val >= maxValue)
                return 1;
            return val / maxValue;
        }


        /// <summary>
        /// Unsigned, normalized integers map into the floating-point range [0, 1.0]. It does this by mapping the entire integer range to that. 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float NormalizeUnsigned(float val, float maxValue)
        {
            if (val <= 0)
                return 0;
            if (val >= maxValue)
                return 1;
            return val / maxValue;
        }
    }
}