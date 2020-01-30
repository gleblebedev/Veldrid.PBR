using System;

namespace Veldrid.PBR
{
    public class SimpleScene:IDisposable
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Swapchain _swapchain;
        private CommandList _cl;

        public SimpleScene(GraphicsDevice graphicsDevice, Swapchain swapchain, PbrContent content)
        {
            _graphicsDevice = graphicsDevice;
            _swapchain = swapchain;
            ResourceFactory = graphicsDevice.ResourceFactory;
            _cl = ResourceFactory.CreateCommandList();
        }

        public ResourceFactory ResourceFactory { get; }

        public void Render(float deltaSeconds)
        {
            _cl.Begin();
            _cl.SetFramebuffer(_swapchain.Framebuffer);
            _cl.SetFullViewport(0);
            _cl.ClearColorTarget(0, RgbaFloat.Blue);
            _cl.End();
            _graphicsDevice.SubmitCommands(_cl);
            _graphicsDevice.SwapBuffers(_swapchain);
        }

        public void Dispose()
        {
            _cl.Dispose();
        }
    }
}