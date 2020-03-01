using System;

namespace Veldrid.PBR
{
    public interface IRenderPipeline : IDisposable
    {
        IMaterial CreateMaterial(MaterialBase material);
    }

    public interface IRenderPipeline<T> : IRenderPipeline
    {
    }
}