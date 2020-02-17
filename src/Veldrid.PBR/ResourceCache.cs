using System;
using System.Collections.Generic;

namespace Veldrid.PBR
{
    public class ResourceCache : IDisposable
    {
        public static readonly ResourceLayoutDescription ProjViewLayoutDescription = new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("ViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex));

        private readonly Dictionary<GraphicsPipelineDescription, Pipeline> s_pipelines
            = new Dictionary<GraphicsPipelineDescription, Pipeline>();

        private readonly Dictionary<ResourceLayoutDescription, ResourceLayout> s_layouts
            = new Dictionary<ResourceLayoutDescription, ResourceLayout>();

        private readonly Dictionary<Texture, TextureView> s_textureViews = new Dictionary<Texture, TextureView>();

        private readonly Dictionary<ResourceSetDescription, ResourceSet> s_resourceSets
            = new Dictionary<ResourceSetDescription, ResourceSet>();

        public ResourceCache(ResourceFactory resourceFactory)
        {
            ResourceFactory = resourceFactory;
        }

        public ResourceFactory ResourceFactory { get; }

        public Pipeline GetPipeline(ref GraphicsPipelineDescription desc)
        {
            if (!s_pipelines.TryGetValue(desc, out var p))
            {
                p = ResourceFactory.CreateGraphicsPipeline(ref desc);
                s_pipelines.Add(desc, p);
            }

            return p;
        }

        public ResourceLayout GetResourceLayout(ResourceLayoutDescription desc)
        {
            if (!s_layouts.TryGetValue(desc, out var p))
            {
                p = ResourceFactory.CreateResourceLayout(ref desc);
                s_layouts.Add(desc, p);
            }

            return p;
        }

        public void Dispose()
        {
            foreach (var kvp in s_pipelines) kvp.Value.Dispose();
            s_pipelines.Clear();

            foreach (var kvp in s_layouts) kvp.Value.Dispose();
            s_layouts.Clear();

            foreach (var kvp in s_textureViews) kvp.Value.Dispose();
            s_textureViews.Clear();

            foreach (var kvp in s_resourceSets) kvp.Value.Dispose();
            s_resourceSets.Clear();
        }

        internal TextureView GetTextureView(Texture texture)
        {
            if (!s_textureViews.TryGetValue(texture, out var view))
            {
                view = ResourceFactory.CreateTextureView(texture);
                s_textureViews.Add(texture, view);
            }

            return view;
        }

        internal ResourceSet GetResourceSet(ResourceSetDescription description)
        {
            if (!s_resourceSets.TryGetValue(description, out var ret))
            {
                ret = ResourceFactory.CreateResourceSet(ref description);
                s_resourceSets.Add(description, ret);
            }

            return ret;
        }
    }
}