using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using SharpGLTF.Schema2;

namespace Veldrid.PBR
{
    internal class Float1VertexAttribute : AbstractVertexAttribute
    {
        private IList<float> _values;

        public Float1VertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsScalarArray();
        }

        public IList<float> Values => _values;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Float1; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write((float)val);
        }
    }
    internal class Float2VertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector2> _values;

        public Float2VertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector2Array();
        }

        public IList<Vector2> Values => _values;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Float2; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write((float)val.X);
            writer.Write((float)val.Y);
        }
    }
    internal class Float3VertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector3> _values;

        public Float3VertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector3Array();
        }

        public IList<Vector3> Values => _values;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Float3; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write((float)val.X);
            writer.Write((float)val.Y);
            writer.Write((float)val.Z);
        }
    }
    internal class Float4VertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector4> _values;

        public Float4VertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector4Array();
        }

        public IList<Vector4> Values => _values;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Float4; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write((float)val.X);
            writer.Write((float)val.Y);
            writer.Write((float)val.Z);
            writer.Write((float)val.W);
        }
    }
    internal class UShort2VertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector2> _values;

        public UShort2VertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector2Array();
        }

        public IList<Vector2> Values => _values;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.UShort2; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write((ushort)val.X);
            writer.Write((ushort)val.Y);
        }
    }
    internal class Byte4_NormVertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector4> _values;

        public Byte4_NormVertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector4Array();
        }

        public IList<Vector4> Values => _values;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Byte4_Norm; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write((float)val.X);
            writer.Write((float)val.Y);
            writer.Write((float)val.Z);
            writer.Write((float)val.W);
        }
    }
    internal class Byte4VertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector4> _values;

        public Byte4VertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector4Array();
        }

        public IList<Vector4> Values => _values;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Byte4; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write((float)val.X);
            writer.Write((float)val.Y);
            writer.Write((float)val.Z);
            writer.Write((float)val.W);
        }
    }
    internal class SByte4_NormVertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector4> _values;

        public SByte4_NormVertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector4Array();
        }

        public IList<Vector4> Values => _values;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.SByte4_Norm; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write((float)val.X);
            writer.Write((float)val.Y);
            writer.Write((float)val.Z);
            writer.Write((float)val.W);
        }
    }
    internal class SByte4VertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector4> _values;

        public SByte4VertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector4Array();
        }

        public IList<Vector4> Values => _values;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.SByte4; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write((float)val.X);
            writer.Write((float)val.Y);
            writer.Write((float)val.Z);
            writer.Write((float)val.W);
        }
    }
}