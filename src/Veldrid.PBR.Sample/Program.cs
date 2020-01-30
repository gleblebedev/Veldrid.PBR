using CommandLine;

namespace Veldrid.PBR.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = Parser.Default.ParseArguments<ViewerOptions>(args) as Parsed<ViewerOptions>;

            var viewerOptions = options?.Value ?? new ViewerOptions();
            VeldridStartupWindow window = new VeldridStartupWindow("glTF Viewer", viewerOptions);

            var content = GltfConverter.ReadGtlf(options.Value.FileName);

            SimpleScene sceneRenderer = null;
            window.GraphicsDeviceCreated += (g,r,s)=>sceneRenderer = new SimpleScene(g,s,content);
            window.GraphicsDeviceDestroyed += () => sceneRenderer?.Dispose();
            window.Rendering += (dt)=>sceneRenderer?.Render(dt);
            window.Run();
        }
    }
}
