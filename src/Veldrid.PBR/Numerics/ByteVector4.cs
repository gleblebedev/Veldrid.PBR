using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Veldrid.PBR.Numerics
{
    /// <summary>
    ///     A structure encapsulating four byte values.
    /// </summary>
    public struct ByteVector4
    {
        /// <summary>
        ///     The X component of the vector.
        /// </summary>
        public byte X;

        /// <summary>
        ///     The Y component of the vector.
        /// </summary>
        public byte Y;

        /// <summary>
        ///     The Z component of the vector.
        /// </summary>
        public byte Z;

        /// <summary>
        ///     The W component of the vector.
        /// </summary>
        public byte W;

        /// <summary>
        ///     Returns the vector (0,0,0,0).
        /// </summary>
        public static ByteVector4 Zero => new ByteVector4();

        public ByteVector4(byte x, byte y, byte z, byte w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static ByteVector4 FromVector4(Vector4 vec)
        {
            return new ByteVector4((byte) vec.X,
                (byte) vec.Y,
                (byte) vec.Z,
                (byte) vec.W);
        }

        public static ByteVector4 FromVector4Norm(Vector4 vec)
        {
            return new ByteVector4(Clamp(vec.X * 255.0f),
                Clamp(vec.Y * 255.0f),
                Clamp(vec.Z * 255.0f),
                Clamp(vec.W * 255.0f));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte Clamp(float value)
        {
            if (value < 0) return 0;
            if (value > 255) return 255;
            return (byte) value;
        }

        /// <summary>
        ///     Returns a String representing this Vector4 instance.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return ToString("G", CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Returns a String representing this Vector4 instance, using the specified format to format individual elements.
        /// </summary>
        /// <param name="format">The format of individual elements.</param>
        /// <returns>The string representation.</returns>
        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Returns a String representing this Vector4 instance, using the specified format to format individual elements
        ///     and the given IFormatProvider.
        /// </summary>
        /// <param name="format">The format of individual elements.</param>
        /// <param name="formatProvider">The format provider to use when formatting elements.</param>
        /// <returns>The string representation.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            var sb = new StringBuilder();
            var separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
            sb.Append('<');
            sb.Append(X.ToString(format, formatProvider));
            sb.Append(separator);
            sb.Append(' ');
            sb.Append(Y.ToString(format, formatProvider));
            sb.Append(separator);
            sb.Append(' ');
            sb.Append(Z.ToString(format, formatProvider));
            sb.Append(separator);
            sb.Append(' ');
            sb.Append(W.ToString(format, formatProvider));
            sb.Append('>');
            return sb.ToString();
        }
    }
}