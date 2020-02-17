using Veldrid.PBR.DataStructures;
using Veldrid.PBR.Unlit;

namespace Veldrid.PBR
{
    public class UnlitTechnique : ITechnique<UnlitMaterial, ImageBasedLightingPasses>
    {
        private readonly IUniformPool<UnlitMaterialArguments> _uniformPool;
        private readonly UnlitShaderFactory _unlitShaderFactory;
        private readonly ImageBasedLighting _renderPipeline;

        public UnlitTechnique(IUniformPool<UnlitMaterialArguments> uniformPool, UnlitShaderFactory unlitShaderFactory,
            ImageBasedLighting renderPipeline)
        {
            _uniformPool = uniformPool;
            _unlitShaderFactory = unlitShaderFactory;
            _renderPipeline = renderPipeline;
        }
        public IMaterialBinding<ImageBasedLightingPasses> BindMaterial(
            UnlitMaterial material,
            PrimitiveTopology topology,
            uint indexCount, uint modelUniformOffset,
            VertexLayoutDescription vertexLayoutDescription)
        {
            var shaders = _unlitShaderFactory.GetOrCreateShaders(new UnlitShaderKey(UnlitShaderFlags.None, vertexLayoutDescription));
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
            description.ShaderSet = new ShaderSetDescription(new VertexLayoutDescription[]{vertexLayoutDescription}, shaders);
            description.ResourceLayouts = _renderPipeline.GetResourceLayouts(ImageBasedLightingPasses.Opaque);
            description.Outputs = _renderPipeline.OutputDescription;
            var pipeline = _renderPipeline.ResourceCache.GetPipeline(ref description);
            return new UnlitMaterialBinding(new MaterialPassBinding(pipeline, indexCount, _renderPipeline.ResourceSet, modelUniformOffset));
        }
    }
}