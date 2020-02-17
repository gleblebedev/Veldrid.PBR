namespace Veldrid.PBR
{
    public interface IMaterialBinding<TPass>
    {
        IPassBinding this[TPass pass] { get; }
    }
}