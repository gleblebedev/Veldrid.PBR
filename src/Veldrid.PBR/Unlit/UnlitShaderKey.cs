using System;

namespace Veldrid.PBR.Unlit
{
    public struct UnlitShaderKey : IEquatable<UnlitShaderKey>
    {
        public UnlitShaderFlags Flags;
        public VertexLayoutDescription Elements;

        public UnlitShaderKey(UnlitShaderFlags flags, VertexLayoutDescription elements)
        {
            Flags = flags;
            Elements = elements;
        }

        public bool Equals(UnlitShaderKey other)
        {
            return Flags == other.Flags && Elements.Equals(other.Elements);
        }

        public override bool Equals(object obj)
        {
            return obj is UnlitShaderKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Flags * 397) ^ Elements.GetHashCode();
            }
        }

        public static bool operator ==(UnlitShaderKey left, UnlitShaderKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UnlitShaderKey left, UnlitShaderKey right)
        {
            return !left.Equals(right);
        }
    }
}