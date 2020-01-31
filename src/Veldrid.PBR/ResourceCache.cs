using System;
using System.Collections.Generic;

namespace Veldrid.PBR
{
    public class ResourceCache: IDisposable
    {
        private readonly ResourceFactory _resourceFactory;

        private readonly Dictionary<GraphicsPipelineDescription, Pipeline> s_pipelines
            = new Dictionary<GraphicsPipelineDescription, Pipeline>();

        private readonly Dictionary<ResourceLayoutDescription, ResourceLayout> s_layouts
            = new Dictionary<ResourceLayoutDescription, ResourceLayout>();

        private readonly Dictionary<Texture, TextureView> s_textureViews = new Dictionary<Texture, TextureView>();

        private readonly Dictionary<ResourceSetDescription, ResourceSet> s_resourceSets
            = new Dictionary<ResourceSetDescription, ResourceSet>();

        public static readonly ResourceLayoutDescription ProjViewLayoutDescription = new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("ViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex));

        public ResourceCache(ResourceFactory resourceFactory)
        {
            _resourceFactory = resourceFactory;
        }

        public Pipeline GetPipeline(ref GraphicsPipelineDescription desc)
        {
            if (!s_pipelines.TryGetValue(desc, out Pipeline p))
            {
                p = _resourceFactory.CreateGraphicsPipeline(ref desc);
                s_pipelines.Add(desc, p);
            }

            return p;
        }

        public ResourceLayout GetResourceLayout(ResourceLayoutDescription desc)
        {
            if (!s_layouts.TryGetValue(desc, out ResourceLayout p))
            {
                p = _resourceFactory.CreateResourceLayout(ref desc);
                s_layouts.Add(desc, p);
            }

            return p;
        }

        public void Dispose()
        {
            foreach (KeyValuePair<GraphicsPipelineDescription, Pipeline> kvp in s_pipelines)
            {
                kvp.Value.Dispose();
            }
            s_pipelines.Clear();

            foreach (KeyValuePair<ResourceLayoutDescription, ResourceLayout> kvp in s_layouts)
            {
                kvp.Value.Dispose();
            }
            s_layouts.Clear();

            foreach (KeyValuePair<Texture, TextureView> kvp in s_textureViews)
            {
                kvp.Value.Dispose();
            }
            s_textureViews.Clear();

            foreach (KeyValuePair<ResourceSetDescription, ResourceSet> kvp in s_resourceSets)
            {
                kvp.Value.Dispose();
            }
            s_resourceSets.Clear();
        }

        internal TextureView GetTextureView(Texture texture)
        {
            if (!s_textureViews.TryGetValue(texture, out TextureView view))
            {
                view = _resourceFactory.CreateTextureView(texture);
                s_textureViews.Add(texture, view);
            }

            return view;
        }

        internal ResourceSet GetResourceSet(ResourceSetDescription description)
        {
            if (!s_resourceSets.TryGetValue(description, out ResourceSet ret))
            {
                ret = _resourceFactory.CreateResourceSet(ref description);
                s_resourceSets.Add(description, ret);
            }

            return ret;
        }
    }
}

