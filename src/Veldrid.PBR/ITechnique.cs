namespace Veldrid.PBR
{
    public interface ITechnique<TMaterial, TPass>
    {
        IMaterialBinding<TPass> BindMaterial(TMaterial material);
    }
}