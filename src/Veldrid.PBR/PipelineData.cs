namespace Veldrid.PBR
{
    public struct PipelineData
    {
        /// <summary>
        /// A description of the blend state, which controls how color values are blended into each color target.
        /// </summary>
        public BlendStateDescription BlendState;
        /// <summary>
        /// A description of the depth stencil state, which controls depth tests, writing, and comparisons.
        /// </summary>
        public DepthStencilStateDescription DepthStencilState;
        /// <summary>
        /// A description of the rasterizer state, which controls culling, clipping, scissor, and polygon-fill behavior.
        /// </summary>
        public RasterizerStateDescription RasterizerState;
        /// <summary>
        /// The <see cref="PrimitiveTopology"/> to use, which controls how a series of input vertices is interpreted by the
        /// <see cref="Pipeline"/>.
        /// </summary>
        public PrimitiveTopology PrimitiveTopology;
    }
}