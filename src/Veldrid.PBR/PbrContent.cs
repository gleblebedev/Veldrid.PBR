using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Veldrid.PBR
{
    public class PbrContent
    {
        internal static readonly Version CurrentVersion = new Version(0, 0, 0, 0);

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

            VersionValue veldridVersion;
            pos = Read(pos, out veldridVersion);
            if (veldridVersion.ToVertsion() != typeof(GraphicsDevice).Assembly.GetName().Version)
                throw new InvalidDataException("Veldrid version doesn't match file");

            VersionValue fileVersion;
            pos = Read(pos, out fileVersion);
            if (fileVersion.ToVertsion() != CurrentVersion)
                throw new InvalidDataException("File format version doesn't match current version");
            pos = Read(pos, out _chunks);
        }

        public int NumBuffers => _chunks.Buffers.Count;
        public int NumMeshes => _chunks.Meshes.Count;
        public int NumNodes => _chunks.Nodes.Count;

        public DeviceBuffer CreateBuffer(int index, GraphicsDevice graphicsDevice, ResourceFactory resourceFactory)
        {
            ref var bufferData = ref _chunks.Buffers.GetAt(_data, index);
            var deviceBuffer = resourceFactory.CreateBuffer(ref bufferData.Description);
            graphicsDevice.UpdateBuffer(deviceBuffer, 0,
                ref _data.Span.Slice(GetBlob(bufferData.BlobIndex)).GetPinnableReference(),
                bufferData.Description.SizeInBytes);
            return deviceBuffer;
        }

        public ref BufferDescription GetBufferDescription(int index)
        {
            return ref _chunks.Buffers.GetAt(_data, index).Description;
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
        private unsafe string GetString(int index)
        {
            var indexRange = _chunks.Strings.GetAt(_data, index);
            var span = MemoryMarshal.Cast<byte, char>(_data.Span.Slice(indexRange.StartIndex, indexRange.Count));
            fixed (char* value = &span.GetPinnableReference())
            {
                return new string(value, 0, indexRange.Count / 2);
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