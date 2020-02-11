using System;

namespace Veldrid.PBR.Unlit
{
    public partial class UnlitPixelShader
    {
        private readonly UnlitShaderKey _key;

        public UnlitPixelShader(UnlitShaderKey key)
        {
            _key = key;
        }
    }

    public partial class UnlitVertexShader
    {
        private readonly UnlitShaderKey _key;

        public UnlitVertexShader(UnlitShaderKey key)
        {
            _key = key;
        }

        public string SpirVTypeFromFormat(VertexElementFormat format)
        {
            switch (format)
            {
                case VertexElementFormat.Float1:
                    return "float";
                case VertexElementFormat.Float2:
                    return "vec2";
                case VertexElementFormat.Float3:
                    return "vec3";
                case VertexElementFormat.Float4:
                case VertexElementFormat.Byte4_Norm:
                case VertexElementFormat.SByte4_Norm:
                case VertexElementFormat.Short4_Norm:
                case VertexElementFormat.UShort4_Norm:
                    return "vec4";
                default:
                    throw new NotImplementedException(format + " is not implemented yet.", null);
            }
        }
    }
}