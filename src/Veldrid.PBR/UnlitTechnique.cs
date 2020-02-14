using System;
using Veldrid.PBR.DataStructures;
using Veldrid.PBR.Unlit;

namespace Veldrid.PBR
{
    public class UnlitTechnique : ITechnique<UnlitMaterial, ImageBasedLightingPasses>
    {
        private readonly IUniformPool<UnlitMaterialArguments> _uniformPool;

        public UnlitTechnique(IUniformPool<UnlitMaterialArguments> uniformPool)
        {
            _uniformPool = uniformPool;
        }
        public IMaterialBinding<ImageBasedLightingPasses> BindMaterial(UnlitMaterial material)
        {
            return new MaterialBinding<ImageBasedLightingPasses>();
        }
    }
}