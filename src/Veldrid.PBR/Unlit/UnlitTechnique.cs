using System.Text.RegularExpressions;
using Veldrid.PBR.ImageBasedLighting;

namespace Veldrid.PBR.Unlit
{
    public class UnlitTechnique : ITechnique<UnlitMaterial, ImageBasedLightingPasses>
    {
        private readonly UnlitShaderFactory _unlitShaderFactory;
        private readonly RenderPipeline _renderPipeline;

        public UnlitTechnique(UnlitShaderFactory unlitShaderFactory,
            RenderPipeline renderPipeline)
        {
            _unlitShaderFactory = unlitShaderFactory;
            _renderPipeline = renderPipeline;
        }

        public IMaterialBinding<ImageBasedLightingPasses> BindMaterial(
            ImageBasedLightingUnlitMaterial material,
            ref PrimitiveDrawCall drawCall)
        {
            var shaders =
                _unlitShaderFactory.GetOrCreateShaders(new UnlitShaderKey(material.ShaderFlags,
                    drawCall.VertexLayoutDescription));
            var description = new GraphicsPipelineDescription();
            description.BlendState = BlendStateDescription.SingleOverrideBlend;
            description.DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual;
            description.PrimitiveTopology = drawCall.Topology;
            description.RasterizerState = new RasterizerStateDescription
            {
                CullMode = FaceCullMode.None,
                FillMode = PolygonFillMode.Solid,
                FrontFace = FrontFace.Clockwise,
                DepthClipEnabled = true,
                ScissorTestEnabled = false
            };
            description.ShaderSet = new ShaderSetDescription(new[] {drawCall.VertexLayoutDescription}, shaders);
            description.ResourceLayouts = new[]
                {_renderPipeline.ModelViewProjectionResourceLayout, material.ResourceLayout};
            description.Outputs = _renderPipeline.OutputDescription;
            var pipeline = _renderPipeline.ResourceCache.GetPipeline(ref description);

            return new UnlitMaterialBinding(new MaterialPassBinding(pipeline, ref drawCall,
                new ResourceSetAndOffsets(_renderPipeline.ModelViewProjectionResourceSet, drawCall.ModelUniformOffset),
                material.ResourceSet));
        }

        public void Dispose()
        {
        }
    }
}