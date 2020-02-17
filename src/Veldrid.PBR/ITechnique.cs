using System;

namespace Veldrid.PBR
{
    public interface ITechnique<TMaterial, TPass> : IDisposable
    {
        IMaterialBinding<TPass> BindMaterial(TMaterial material, PrimitiveTopology topology, uint indexCount,
            uint modelUniformOffset, VertexLayoutDescription vertexLayoutDescription);
    }
}