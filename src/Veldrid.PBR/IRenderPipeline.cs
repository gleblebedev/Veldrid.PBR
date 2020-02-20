using System;

namespace Veldrid.PBR
{
    public interface IRenderPipeline : IDisposable
    {
        IMaterial CreateMaterial(UnlitMaterial unlitMaterial);
    }

    public interface IRenderPipeline<T> : IRenderPipeline
    {
    }
}