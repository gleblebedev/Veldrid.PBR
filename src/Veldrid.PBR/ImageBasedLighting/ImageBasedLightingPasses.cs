namespace Veldrid.PBR.ImageBasedLighting
{
    /// <summary>
    ///     Render passes for PBR IBL.
    ///     There is only one render pass required for IBL as all lighting and reflection information is baked into a single
    ///     cubemap.
    /// </summary>
    public enum ImageBasedLightingPasses
    {
        Opaque
    }
}