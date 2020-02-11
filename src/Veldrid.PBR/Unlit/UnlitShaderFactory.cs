using System.Collections.Generic;
using System.Text;
using Veldrid.SPIRV;

namespace Veldrid.PBR.Unlit
{
    public class UnlitShaderFactory: IShaderFactory<UnlitShaderKey>
    {
        private readonly Veldrid.ResourceFactory _resourceFactory;
        private Dictionary<UnlitShaderKey, Shader[]> _shaders =new Dictionary<UnlitShaderKey, Shader[]>();

        public UnlitShaderFactory(Veldrid.ResourceFactory resourceFactory)
        {
            _resourceFactory = resourceFactory;
        }

        public Shader[] GetOrCreateShaders(UnlitShaderKey key)
        {
            if (!_shaders.TryGetValue(key, out var shaders))
            {
                shaders = _resourceFactory.CreateFromSpirv(new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(new UnlitVertexShader(key).TransformText()), "main"),
                    new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(new UnlitPixelShader(key).TransformText()), "main"));
                _shaders.Add(key, shaders);
            }

            return shaders;
        }

        public void Dispose()
        {
            foreach (var shaders in _shaders)
            {
                foreach (var shader in shaders.Value)
                {
                    shader.Dispose();
                }
            }
        }
    }
}