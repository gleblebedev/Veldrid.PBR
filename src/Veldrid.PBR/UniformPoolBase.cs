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
        private DeviceBufferRange _bindableResource;

        public UniformPoolBase(GraphicsDevice graphicsDevice, uint elementSize, uint capacity)
        {
            _graphicsDevice = graphicsDevice;
            _elementSize = elementSize;
            _alignment = graphicsDevice.UniformBufferMinOffsetAlignment;
            _stride = _alignment * ((_elementSize + _alignment - 1) / _elementSize);
            _buffer = graphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription(_stride * capacity, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _bindableResource = new DeviceBufferRange(_buffer, 0, _stride);
            _availabilityMask = new BitMask(capacity);
        }

        public DeviceBuffer DeviceBuffer => _buffer;

        public BindableResource BindableResource => _bindableResource;

        public virtual void Dispose()
        {
            _buffer.Dispose();
        }

        /// <summary>
        /// Allocate portion of the uniform buffer.
        /// </summary>
        /// <returns>Offset in the buffer for the allocated data.</returns>
        public uint Allocate()
        {
            var index = _availabilityMask.FindFirstAvailableBit();
            _availabilityMask.SetAt(index);
            return index * _stride;
        }

        public void Release(uint offset)
        {
            _availabilityMask[offset/_stride] = true;
        }

        protected void Upload<T>(uint offset, ref T value) where T : struct
        {
            _graphicsDevice.UpdateBuffer(_buffer, offset, ref value);
        }
    }
}