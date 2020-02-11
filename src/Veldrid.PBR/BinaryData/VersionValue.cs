using System;

namespace Veldrid.PBR.BinaryData
{
    public struct VersionValue : IEquatable<VersionValue>
    {
        public int Major;
        public int Minor;
        public int Build;
        public int Revision;

        public VersionValue(Version version)
        {
            Major = version.Major;
            Minor = version.Minor;
            Revision = version.Revision;
            Build = version.Build;
        }

        public Version ToVertsion()
        {
            return new Version(Major, Minor, Build, Revision);
        }

        public bool Equals(VersionValue other)
        {
            return Major == other.Major && Minor == other.Minor && Build == other.Build && Revision == other.Revision;
        }

        public override bool Equals(object obj)
        {
            return obj is VersionValue other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Major;
                hashCode = (hashCode * 397) ^ Minor;
                hashCode = (hashCode * 397) ^ Build;
                hashCode = (hashCode * 397) ^ Revision;
                return hashCode;
            }
        }

        public static bool operator ==(VersionValue left, VersionValue right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VersionValue left, VersionValue right)
        {
            return !left.Equals(right);
        }
    }
}