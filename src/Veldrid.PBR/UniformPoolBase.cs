namespace Veldrid.PBR
{
    public class UniformPoolBase:IUniformPool
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly uint _elementSize;
        private uint _alignment;
        private uint _stride;
        private DeviceBuffer _buffer;
        private BitMask _availabilityMask;

        public UniformPoolBase(GraphicsDevice graphicsDevice, uint elementSize, uint capacity)
        {
            _graphicsDevice = graphicsDevice;
            _elementSize = elementSize;
            _alignment = graphicsDevice.StructuredBufferMinOffsetAlignment;
            _stride = _alignment * ((_elementSize + _alignment - 1) / _elementSize);
            _buffer = graphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription(_stride * capacity, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _availabilityMask = new BitMask(capacity);
        }

        public virtual void Dispose()
        {
            _buffer.Dispose();
        }

        public uint Allocate()
        {
            var index = _availabilityMask.FindFirstAvailableBit();
            _availabilityMask.SetAt(index);
            return index;
        }

        public void Release(uint index)
        {
            _availabilityMask[index] = true;
        }

        protected void Upload<T>(uint index, ref T value) where T : struct
        {
            _graphicsDevice.UpdateBuffer(_buffer, _stride*index, ref value);
        }
    }
}