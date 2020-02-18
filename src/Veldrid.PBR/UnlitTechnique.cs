using Veldrid.PBR.Unlit;

namespace Veldrid.PBR
{
    public class UnlitTechnique : ITechnique<UnlitMaterial, ImageBasedLightingPasses>
    {
        private readonly UnlitShaderFactory _unlitShaderFactory;
        private readonly ImageBasedLighting _renderPipeline;

        public UnlitTechnique(UnlitShaderFactory unlitShaderFactory,
            ImageBasedLighting renderPipeline)
        {
            _unlitShaderFactory = unlitShaderFactory;
            _renderPipeline = renderPipeline;
        }

        public void Dispose()
        {
        }

        public IMaterialBinding<ImageBasedLightingPasses> BindMaterial(
            uint materialOffset,
            PrimitiveTopology topology,
            uint indexCount, uint modelUniformOffset,
            VertexLayoutDescription vertexLayoutDescription)
        {
            var shaders =
                _unlitShaderFactory.GetOrCreateShaders(new UnlitShaderKey(UnlitShaderFlags.None,
                    vertexLayoutDescription));
            var description = new GraphicsPipelineDescription();
            description.BlendState = BlendStateDescription.SingleOverrideBlend;
            description.DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual;
            description.PrimitiveTopology = topology;
            description.RasterizerState = new RasterizerStateDescription
            {
                CullMode = FaceCullMode.None,
                FillMode = PolygonFillMode.Solid,
                FrontFace = FrontFace.Clockwise,
                DepthClipEnabled = true,
                ScissorTestEnabled = false
            };
            description.ShaderSet = new ShaderSetDescription(new[] {vertexLayoutDescription}, shaders);
            description.ResourceLayouts = _renderPipeline.GetResourceLayouts(ImageBasedLightingPasses.Opaque);
            description.Outputs = _renderPipeline.OutputDescription;
            var pipeline = _renderPipeline.ResourceCache.GetPipeline(ref description);
            return new UnlitMaterialBinding(new MaterialPassBinding(pipeline, indexCount, 
                _renderPipeline.ModelViewProjectionResourceSet, 
                _renderPipeline.UnlitMaterialResourceSet,
                modelUniformOffset, 
                materialOffset));
        }
    }
}