using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Veldrid.PBR
{
    public class PbrContent
    {
        internal static readonly Version CurrentVersion = new Version(0, 0, 0, 0);
        internal static readonly byte[] Magic = Encoding.ASCII.GetBytes("VELDRID.PBR.MESH");

        private readonly Memory<byte> _data;
        private Lumps _lumps;

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
            pos = Read(pos, out _lumps);
        }

        public int NumBuffers => _lumps.Buffers.Count;
        public int NumMeshes => _lumps.Meshes.Count;

        public DeviceBuffer CreateBuffer(int index, GraphicsDevice graphicsDevice, ResourceFactory resourceFactory)
        {
            ref var bufferData = ref _lumps.Buffers.GetAt(_data, index);
            var deviceBuffer = resourceFactory.CreateBuffer(ref bufferData.Description);
            graphicsDevice.UpdateBuffer(deviceBuffer, 0, ref _data.Span.Slice(bufferData.Offset).GetPinnableReference(),
                bufferData.Description.SizeInBytes);
            return deviceBuffer;
        }

        public ref BufferDescription GetBufferDescription(int index)
        {
            return ref _lumps.Buffers.GetAt(_data, index).Description;
        }


        public ref MeshData GetMesh(int index)
        {
            return ref _lumps.Meshes.GetAt(_data, index);
        }

        public ref PrimitiveData GetPrimitive(int index)
        {
            return ref _lumps.Primitives.GetAt(_data, index);
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