using System;

namespace Veldrid.PBR
{
    public class ImageBasedLightingTechnique : ITechnique<PhysicallyBasedMaterial, ImageBasedLightingPasses>
    {
        public IMaterialBinding<ImageBasedLightingPasses> BindMaterial(PhysicallyBasedMaterial material)
        {
            throw new NotImplementedException();
        }
    }
}