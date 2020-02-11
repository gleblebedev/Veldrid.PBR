using System.Collections.Generic;
using System.Text;
using Veldrid.SPIRV;

namespace Veldrid.PBR.Unlit
{
    public class UnlitShaderFactory : IShaderFactory<UnlitShaderKey>
    {
        private readonly ResourceFactory _resourceFactory;
        private readonly Dictionary<UnlitShaderKey, Shader[]> _shaders = new Dictionary<UnlitShaderKey, Shader[]>();

        public UnlitShaderFactory(ResourceFactory resourceFactory)
        {
            _resourceFactory = resourceFactory;
        }

        public void Dispose()
        {
            foreach (var shaders in _shaders)
            foreach (var shader in shaders.Value)
                shader.Dispose();
        }

        public Shader[] GetOrCreateShaders(UnlitShaderKey key)
        {
            if (!_shaders.TryGetValue(key, out var shaders))
            {
                var vertexShader = new UnlitVertexShader(key).TransformText();
                var fragmentShader = new UnlitPixelShader(key).TransformText();
                shaders = _resourceFactory.CreateFromSpirv(
                    new ShaderDescription(ShaderStages.Vertex,
                        Encoding.UTF8.GetBytes(vertexShader), "main"),
                    new ShaderDescription(ShaderStages.Fragment,
                        Encoding.UTF8.GetBytes(fragmentShader), "main"));
                _shaders.Add(key, shaders);
            }

            return shaders;
        }
    }
}