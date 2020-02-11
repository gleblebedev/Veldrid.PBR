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

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Float1; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write((val));
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

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Float2; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write((val.X));
            writer.Write((val.Y));
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

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Float3; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write((val.X));
            writer.Write((val.Y));
            writer.Write((val.Z));
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

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Float4; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write((val.X));
            writer.Write((val.Y));
            writer.Write((val.Z));
            writer.Write((val.W));
        }
    }
    internal class Byte2_NormVertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector2> _values;

        public Byte2_NormVertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector2Array();
        }

        public IList<Vector2> Values => _values;

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Byte2_Norm; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(FloatToNormByte(val.X));
            writer.Write(FloatToNormByte(val.Y));
        }
    }
    internal class Byte2VertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector2> _values;

        public Byte2VertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector2Array();
        }

        public IList<Vector2> Values => _values;

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Byte2; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(FloatToByte(val.X));
            writer.Write(FloatToByte(val.Y));
        }
    }
    internal class SByte2_NormVertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector2> _values;

        public SByte2_NormVertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector2Array();
        }

        public IList<Vector2> Values => _values;

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.SByte2_Norm; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(FloatToNormSByte(val.X));
            writer.Write(FloatToNormSByte(val.Y));
        }
    }
    internal class SByte2VertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector2> _values;

        public SByte2VertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector2Array();
        }

        public IList<Vector2> Values => _values;

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.SByte2; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(FloatToSByte(val.X));
            writer.Write(FloatToSByte(val.Y));
        }
    }
    internal class Short2_NormVertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector2> _values;

        public Short2_NormVertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector2Array();
        }

        public IList<Vector2> Values => _values;

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Short2_Norm; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(FloatToNormShort(val.X));
            writer.Write(FloatToNormShort(val.Y));
        }
    }
    internal class Short2VertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector2> _values;

        public Short2VertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector2Array();
        }

        public IList<Vector2> Values => _values;

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Short2; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(FloatToShort(val.X));
            writer.Write(FloatToShort(val.Y));
        }
    }
    internal class UShort2_NormVertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector2> _values;

        public UShort2_NormVertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector2Array();
        }

        public IList<Vector2> Values => _values;

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.UShort2_Norm; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(FloatToNormUShort(val.X));
            writer.Write(FloatToNormUShort(val.Y));
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

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.UShort2; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(FloatToUShort(val.X));
            writer.Write(FloatToUShort(val.Y));
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

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Byte4_Norm; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(FloatToNormByte(val.X));
            writer.Write(FloatToNormByte(val.Y));
            writer.Write(FloatToNormByte(val.Z));
            writer.Write(FloatToNormByte(val.W));
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

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Byte4; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(FloatToByte(val.X));
            writer.Write(FloatToByte(val.Y));
            writer.Write(FloatToByte(val.Z));
            writer.Write(FloatToByte(val.W));
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

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.SByte4_Norm; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(FloatToNormSByte(val.X));
            writer.Write(FloatToNormSByte(val.Y));
            writer.Write(FloatToNormSByte(val.Z));
            writer.Write(FloatToNormSByte(val.W));
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

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.SByte4; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(FloatToSByte(val.X));
            writer.Write(FloatToSByte(val.Y));
            writer.Write(FloatToSByte(val.Z));
            writer.Write(FloatToSByte(val.W));
        }
    }
    internal class Short4_NormVertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector4> _values;

        public Short4_NormVertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector4Array();
        }

        public IList<Vector4> Values => _values;

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Short4_Norm; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(FloatToNormShort(val.X));
            writer.Write(FloatToNormShort(val.Y));
            writer.Write(FloatToNormShort(val.Z));
            writer.Write(FloatToNormShort(val.W));
        }
    }
    internal class Short4VertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector4> _values;

        public Short4VertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector4Array();
        }

        public IList<Vector4> Values => _values;

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.Short4; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(FloatToShort(val.X));
            writer.Write(FloatToShort(val.Y));
            writer.Write(FloatToShort(val.Z));
            writer.Write(FloatToShort(val.W));
        }
    }
    internal class UShort4_NormVertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector4> _values;

        public UShort4_NormVertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector4Array();
        }

        public IList<Vector4> Values => _values;

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.UShort4_Norm; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(FloatToNormUShort(val.X));
            writer.Write(FloatToNormUShort(val.Y));
            writer.Write(FloatToNormUShort(val.Z));
            writer.Write(FloatToNormUShort(val.W));
        }
    }
    internal class UShort4VertexAttribute : AbstractVertexAttribute
    {
        private IList<Vector4> _values;

        public UShort4VertexAttribute(string key, Accessor accessor): base(key)
        {
            _values = accessor.AsVector4Array();
        }

        public IList<Vector4> Values => _values;

        public override int Count => _values.Count;

        public override VertexElementFormat VertexElementFormat { get { return VertexElementFormat.UShort4; } }

        public override void Write(BinaryWriter writer, int index)
        {
            var val = _values[index];
            writer.Write(FloatToUShort(val.X));
            writer.Write(FloatToUShort(val.Y));
            writer.Write(FloatToUShort(val.Z));
            writer.Write(FloatToUShort(val.W));
        }
    }
}