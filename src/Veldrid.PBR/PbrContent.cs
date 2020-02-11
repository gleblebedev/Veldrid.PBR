using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid.PBR.BinaryData;

namespace Veldrid.PBR
{
    public class PbrContent
    {
        internal static readonly VersionValue CurrentVersion = new VersionValue(new Version(0, 0, 0, 0));

        internal static readonly byte[] Magic =
            Encoding.ASCII.GetBytes("VELDRIDPBR" + (BitConverter.IsLittleEndian ? "LE" : "BE"));

        private readonly Memory<byte> _data;
        private Chunks _chunks;

        public PbrContent(Memory<byte> data)
        {
            _data = data;
            var pos = 0;
            var dataSpan = data.Span;
            for (; pos < Magic.Length; ++pos)
                if (dataSpan[pos] != Magic[pos])
                    throw new InvalidDataException("Magic doesn't match");

            VersionValue fileVersion;
            pos = Read(pos, out fileVersion);
            if (fileVersion != CurrentVersion)
                throw new InvalidDataException("File format version doesn't match current version");
            pos = Read(pos, out _chunks);
        }

        public int NumBuffers => _chunks.Buffers.Count;
        public int NumMeshes => _chunks.Meshes.Count;
        public int NumNodes => _chunks.Nodes.Count;

        public DeviceBuffer CreateBuffer(int index, GraphicsDevice graphicsDevice, ResourceFactory resourceFactory)
        {
            ref var bufferData = ref _chunks.Buffers.GetAt(_data, index);
            GetBufferDescription(index, out var bufferDescription);
            var deviceBuffer = resourceFactory.CreateBuffer(ref bufferDescription);
            graphicsDevice.UpdateBuffer(deviceBuffer, 0,
                ref _data.Span.Slice(GetBlob(bufferData.BlobIndex)).GetPinnableReference(),
                bufferDescription.SizeInBytes);
            return deviceBuffer;
        }

        public void CreateSamplerDescription(int index, out SamplerDescription description)
        {
            ref var samplerData = ref _chunks.Sampler.GetAt(_data, index);
            description = new SamplerDescription(
                samplerData.AddressModeU,
                samplerData.AddressModeV,
                samplerData.AddressModeW,
                samplerData.Filter,
                samplerData.ComparisonKind,
                samplerData.MaximumAnisotropy,
                samplerData.MinimumLod,
                samplerData.MaximumLod,
                samplerData.LodBias,
                samplerData.BorderColor);
        }

        public Sampler CreateSampler(int index, GraphicsDevice graphicsDevice, ResourceFactory resourceFactory)
        {
            CreateSamplerDescription(index, out var description);
            return resourceFactory.CreateSampler(description);
        }

        private void GetBufferDescription(int index, out BufferDescription description)
        {
            ref var bufferData = ref _chunks.Buffers.GetAt(_data, index);
            ref var blob = ref _chunks.BinaryBlobs.GetAt(_data, bufferData.BlobIndex);
            description.SizeInBytes = (uint)blob.Count;
            description.Usage = bufferData.Usage;
            description.StructureByteStride = bufferData.StructureByteStride;
            description.RawBuffer = bufferData.RawBuffer;
        }

        public ref VertexBufferViewData GetVertexBufferView(int index)
        {
            return ref _chunks.BufferViews.GetAt(_data, index);
        }

        public ref MeshData GetMesh(int index)
        {
            return ref _chunks.Meshes.GetAt(_data, index);
        }

        public ref NodeData GetNode(int index)
        {
            return ref _chunks.Nodes.GetAt(_data, index);
        }

        public ref PrimitiveData GetPrimitive(int index)
        {
            return ref _chunks.Primitives.GetAt(_data, index);
        }


        public VertexLayoutDescription GetVertexLayoutDescription(IndexRange elements)
        {
            var res = new VertexElementDescription[elements.Count];
            for (var index = 0; index < res.Length; index++)
            {
                ref var vertexElementData = ref _chunks.VertexElements.GetAt(_data, elements.StartIndex + index);
                res[index] = new VertexElementDescription(GetString(vertexElementData.Name), vertexElementData.Semantic,
                    vertexElementData.Format, vertexElementData.Offset);
            }

            return new VertexLayoutDescription(res);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IndexRange GetBlob(int index)
        {
            return _chunks.BinaryBlobs.GetAt(_data, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Span<char> GetStringView(int index)
        {
            var indexRange = _chunks.Strings.GetAt(_data, index);
            return MemoryMarshal.Cast<byte, char>(_data.Span.Slice(indexRange.StartIndex, indexRange.Count));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe string GetString(int index)
        {
            var span = GetStringView(index);
            fixed (char* value = &span.GetPinnableReference())
            {
                return new string(value, 0, span.Length);
            }
        }

        private int Read<T>(int pos, out T value) where T : struct
        {
            var size = Marshal.SizeOf<T>();
            value = MemoryMarshal.Read<T>(_data.Span.Slice(pos, size));
            return pos + size;
        }

        private int Read<T>(int pos, int index, out T value) where T : struct
        {
            var size = Marshal.SizeOf<T>();
            value = MemoryMarshal.Read<T>(_data.Span.Slice(pos + index * size, size));
            return pos + size;
        }
    }
}