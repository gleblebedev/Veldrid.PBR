using System;
using System.Collections.Generic;
using System.Text;

namespace Veldrid.PBR
{
    public interface IUniformPool:IDisposable
    {
        uint Allocate();
        void Release(uint index);
    }
    public interface IUniformPool<T>: IUniformPool
    {
        void UpdateBuffer(uint index, ref T value);
    }
}
