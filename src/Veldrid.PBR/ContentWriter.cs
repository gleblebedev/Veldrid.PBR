using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;

namespace Veldrid.PBR
{
    public class ContentWriter : IDisposable
    {
        private const int MaxStructSize = 512;
        private readonly Stream _writer;
        private readonly bool _disposeWriter;
        private byte[] _buffer;

        public ContentWriter(Stream stream, bool disposeStream = false)
        {
            _writer = stream;
            _disposeWriter = disposeStream;
            _buffer = ArrayPool<byte>.Shared.Rent(MaxStructSize);
        }

        public int Position => (int) _writer.Position;

        public void Write(ContentToWrite content)
        {
            if (content.BufferData.Count != content.BufferDescription.Count)
                throw new ArgumentException("BufferDescription count doesn't match BufferData count");
            Write(PbrContent.Magic);
            Write(new VersionValue(typeof(GraphicsDevice).Assembly.GetName().Version));
            Write(new VersionValue(PbrContent.CurrentVersion));
            var lumps = new Lumps();
            var lumpPos = Position;
            Write(ref lumps);

            var bufferDataValues = new BufferData[content.BufferData.Count];
            for (var index = 0; index < content.BufferData.Count; index++)
            {
                var bufferData = new BufferData {Description = content.BufferDescription[index]};
                if (bufferData.Description.SizeInBytes == 0)
                    bufferData.Description.SizeInBytes = (uint) content.BufferData[index].Length;
                bufferData.Offset = Position;
                bufferDataValues[index] = bufferData;
                Write(content.BufferData[index]);
            }

            lumps.Buffers.Offset = Position;
            lumps.Buffers.Count = content.BufferData.Count;

            for (var index = 0; index < content.BufferData.Count; index++) Write(bufferDataValues[index]);

            lumps.Primitives.Offset = Position;
            lumps.Primitives.Count = content.Primitive.Count;
            for (var index = 0; index < content.Primitive.Count; index++) Write(content.Primitive[index]);

            lumps.Meshes.Offset = Position;
            lumps.Meshes.Count = content.Mesh.Count;
            for (var index = 0; index < content.Mesh.Count; index++) Write(content.Mesh[index]);


            lumps.Nodes.Offset = Position;
            lumps.Nodes.Count = content.Node.Count;
            for (var index = 0; index < content.Node.Count; index++) Write(content.Node[index]);

            _writer.Position = lumpPos;
            Write(ref lumps);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _writer.Write(buffer, offset, count);
        }

        public void Dispose()
        {
            if (_disposeWriter)
                _writer.Dispose();
            ArrayPool<byte>.Shared.Return(_buffer);
        }

        private void Write(byte[] buffer)
        {
            _writer.Write(buffer, 0, buffer.Length);
        }

        private void Write<T>(ref T value) where T : struct
        {
            var size = Marshal.SizeOf<T>();
            if (size > _buffer.Length)
            {
                ArrayPool<byte>.Shared.Return(_buffer);
                _buffer = ArrayPool<byte>.Shared.Rent(size);
            }

            MemoryMarshal.Write(_buffer.AsSpan(0, size), ref value);
            Write(_buffer, 0, size);
        }

        private void Write<T>(T value) where T : struct
        {
            Write(ref value);
        }
    }
}