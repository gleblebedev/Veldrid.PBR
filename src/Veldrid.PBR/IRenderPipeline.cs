using System;

namespace Veldrid.PBR
{
    public interface IRenderPipeline: IDisposable
    {

    }
    public interface IRenderPipeline<T>:IRenderPipeline
    {
        ResourceLayout[] GetResourceLayouts(T pass);
    }
}