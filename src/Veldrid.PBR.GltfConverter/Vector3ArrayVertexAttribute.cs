using System.Collections;
using System.IO;
using System.Numerics;

namespace Veldrid.PBR
{
    internal class Vector3ArrayVertexAttribute : AbstractVertexAttribute
    {
        private readonly Vector3[] _values;

        public Vector3ArrayVertexAttribute(string key, Vector3[] _values) : base(key)
        {
            this._values = _values;
        }

        public override VertexElementFormat VertexElementFormat => VertexElementFormat.Float3;
        public override int Count => _values.Length;
        public Vector3[] Values => _values;

        public override void Write(BinaryWriter vertexWriter, int index)
        {
            vertexWriter.Write(_values[index].X);
            vertexWriter.Write(_values[index].Y);
            vertexWriter.Write(_values[index].Z);
        }
    }
}