namespace Veldrid.PBR.Unlit
{
    public class UnlitMaterialBinding : IMaterialBinding<ImageBasedLightingPasses>
    {
        private readonly MaterialPassBinding[] _bindings;

        public UnlitMaterialBinding(params MaterialPassBinding[] bindings)
        {
            _bindings = bindings;
        }

        public IPassBinding this[ImageBasedLightingPasses pass] => _bindings[(int) pass];
    }
}