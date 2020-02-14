using System.Runtime.InteropServices;

namespace Veldrid.PBR
{
    public class SimpleUniformPool<T>: UniformPoolBase, IUniformPool<T> where T:struct
    {

        public SimpleUniformPool(uint capacity, GraphicsDevice graphicsDevice):base(graphicsDevice, (uint)Marshal.SizeOf<T>(), capacity)
        {
        }

        public void UpdateBuffer(uint index, ref T value)
        {
            base.Upload(index, ref value);
        }
    }
}