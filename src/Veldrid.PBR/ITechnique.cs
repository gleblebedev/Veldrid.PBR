using System;

namespace Veldrid.PBR
{
    public interface ITechnique<TMaterial, TPass> : IDisposable
    {
        //IMaterialBinding<TPass> BindMaterial(ResourceLayout materialLayout,
        //    in ResourceSetAndOffsets materialResourceSet, PrimitiveTopology topology, uint indexCount,
        //    uint modelUniformOffset, VertexLayoutDescription vertexLayoutDescription);
    }
}