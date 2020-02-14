using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Veldrid.PBR.DataStructures
{
    public partial struct MapUV
    {
        public static readonly MapUV Default = new MapUV() {Set = 0, X = Vector3.UnitX, Y = Vector3.UnitY };
    }
}