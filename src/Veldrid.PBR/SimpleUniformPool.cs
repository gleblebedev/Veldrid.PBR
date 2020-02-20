using System.Runtime.CompilerServices;

namespace Veldrid.PBR
{
    public class SimpleUniformPool<T> : UniformPoolBase, IUniformPool<T> where T : struct
    {
        public SimpleUniformPool(uint capacity, GraphicsDevice graphicsDevice) : base(graphicsDevice,
            (uint) Unsafe.SizeOf<T>(), capacity)
        {
        }

        public void UpdateBuffer(uint offset, ref T value)
        {
            Upload(offset, ref value);
        }
    }
}