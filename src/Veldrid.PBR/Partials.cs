using System.Numerics;

namespace Veldrid.PBR.DataStructures
{
    public partial struct MapUV
    {
        public static readonly MapUV Default = new MapUV {Set = 0, X = Vector3.UnitX, Y = Vector3.UnitY};
    }
}