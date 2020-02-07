namespace Veldrid.PBR
{
    public interface ITechnique<TMaterial, TPass>
    {
        IMaterialBinding<TPass> BindMaterial(TMaterial material);
    }

    public interface IMaterialBinding<TPass>
    {
        IPassBinding this[TPass pass] { get; }
    }
}