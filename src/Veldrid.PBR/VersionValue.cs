using System;

namespace Veldrid.PBR
{
    public struct VersionValue
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
    }
}