using System;

namespace Veldrid.PBR
{
    public class UnlitTechnique : ITechnique<UnlitMaterial, ImageBasedLightingPasses>
    {
        public IMaterialBinding<ImageBasedLightingPasses> BindMaterial(UnlitMaterial material)
        {
            throw new NotImplementedException();
        }
    }
}