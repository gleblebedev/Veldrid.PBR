using System;

namespace Veldrid.PBR
{
    public interface IShaderFactory<TKey> : IDisposable
    {
        Shader[] GetOrCreateShaders(TKey key);
    }
}