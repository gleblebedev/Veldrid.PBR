using System;

namespace Veldrid.PBR
{
    public interface IUniformPool : IDisposable
    {
        BindableResource BindableResource { get; }

        /// <summary>
        ///     Allocate portion of the uniform buffer.
        /// </summary>
        /// <returns>Offset in the buffer for the allocated data.</returns>
        uint Allocate();

        void Release(uint offset);
    }

    public interface IUniformPool<T> : IUniformPool
    {
        void UpdateBuffer(uint offset, ref T value);
    }
}