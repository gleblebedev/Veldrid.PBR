using System.Collections.Generic;
using System.IO;
using System.Numerics;
using SharpGLTF.Schema2;

namespace Veldrid.PBR
{
    internal class Float1VertexAttribute : AbstractVertexAttribute
    {
        private readonly string _key;
        private readonly IList<float> _values;

        public Float1VertexAttribute(string key, Accessor accessor)
        {
            _key = key;
            _values = accessor.AsScalarArray();
        }

        public override VertexElementFormat VertexElementFormat => VertexElementFormat.Float1;

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(val);
        }
    }

    internal class Float2VertexAttribute : AbstractVertexAttribute
    {
        private readonly string _key;
        private readonly IList<Vector2> _values;

        public Float2VertexAttribute(string key, Accessor accessor)
        {
            _key = key;
            _values = accessor.AsVector2Array();
        }

        public override VertexElementFormat VertexElementFormat => VertexElementFormat.Float2;

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(val.X);
            writer.Write(val.Y);
        }
    }

    internal class Float3VertexAttribute : AbstractVertexAttribute
    {
        private readonly string _key;
        private readonly IList<Vector3> _values;

        public Float3VertexAttribute(string key, Accessor accessor)
        {
            _key = key;
            _values = accessor.AsVector3Array();
        }

        public override VertexElementFormat VertexElementFormat => VertexElementFormat.Float3;

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(val.X);
            writer.Write(val.Y);
            writer.Write(val.Z);
        }
    }

    internal class Float4VertexAttribute : AbstractVertexAttribute
    {
        private readonly string _key;
        private readonly IList<Vector4> _values;

        public Float4VertexAttribute(string key, Accessor accessor)
        {
            _key = key;
            _values = accessor.AsVector4Array();
        }

        public override VertexElementFormat VertexElementFormat => VertexElementFormat.Float4;

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(val.X);
            writer.Write(val.Y);
            writer.Write(val.Z);
            writer.Write(val.W);
        }
    }
}