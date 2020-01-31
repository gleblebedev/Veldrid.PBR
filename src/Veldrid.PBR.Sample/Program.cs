using CommandLine;

namespace Veldrid.PBR.Sample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var options = Parser.Default.ParseArguments<ViewerOptions>(args) as Parsed<ViewerOptions>;

            var viewerOptions = options?.Value ?? new ViewerOptions();
            var window = new VeldridStartupWindow("Veldrid.PBR Sample", viewerOptions);

            var content = GltfConverter.ReadGtlf(options.Value.FileName);

            SimpleScene sceneRenderer = null;
            ResourceCache resourceCache = null;
            window.GraphicsDeviceCreated += (g, r, s) =>
            {
                resourceCache = new ResourceCache(g.ResourceFactory);
                sceneRenderer = new SimpleScene(g, s, resourceCache, content);
            };
            window.GraphicsDeviceDestroyed += () =>
            {
                sceneRenderer?.Dispose();
                resourceCache?.Dispose();
            };
            window.Rendering += dt => sceneRenderer?.Render(dt);
            window.Run();
        }
    }
}