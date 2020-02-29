using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid.PBR.BinaryData;

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
            if (Position != 0)
                throw new NotImplementedException("Stream should be at 0 position.");
        }

        public int Position => (int) _writer.Position;

        public void Write(ContentToWrite content)
        {
            Write(PbrContent.Magic);
            Write(PbrContent.CurrentVersion);
            var lumps = new Chunks();
            var lumpPos = Position;
            Write(ref lumps);

            {
                // Saving binary blobs and strings
                var binaryBlobs = new IndexRange[content.BinaryBlobs.Count];
                for (var index = 0; index < content.BinaryBlobs.Count; index++)
                {
                    var contentBinaryBlob = content.BinaryBlobs[index];
                    binaryBlobs[index] = new IndexRange(Position, contentBinaryBlob.Count);
                    Write(contentBinaryBlob.Array, contentBinaryBlob.Offset, contentBinaryBlob.Count);
                }

                lumps.BinaryBlobs.Offset = Position;
                lumps.BinaryBlobs.Count = content.BinaryBlobs.Count;
                Write(binaryBlobs);
            }

            {
                var strings = new IndexRange[content.Strings.Count];
                for (var index = 0; index < content.Strings.Count; index++)
                {
                    var str = content.Strings[index];
                    var buffer = Encoding.Unicode.GetBytes(str);
                    strings[index] = new IndexRange(Position, buffer.Length);
                    Write(buffer, 0, buffer.Length);
                }

                lumps.Strings.Offset = Position;
                lumps.Strings.Count = strings.Length;
                Write(strings);
            }

            lumps.Buffers.Offset = Position;
            lumps.Buffers.Count = content.Buffers.Count;
            for (var index = 0; index < content.Buffers.Count; index++)
            {
                var bufferData = content.Buffers[index];
                if (bufferData.BlobIndex < 0 || bufferData.BlobIndex >= content.BinaryBlobs.Count)
                    throw new IndexOutOfRangeException(
                        $"BlobIndex {bufferData.BlobIndex} doesn't match number of binary blobs {content.BinaryBlobs.Count}.");
                Write(bufferData);
            }

            lumps.Textures.Offset = Position;
            lumps.Textures.Count = content.Textures.Count;

            for (var index = 0; index < content.Textures.Count; index++)
            {
                var texture = content.Textures[index];
                if (texture.BlobIndex < 0 || texture.BlobIndex >= content.BinaryBlobs.Count)
                    throw new IndexOutOfRangeException(
                        $"BlobIndex {texture.BlobIndex} doesn't match number of binary blobs {content.BinaryBlobs.Count}.");
                Write(texture);
            }

            lumps.Samplers.Offset = Position;
            lumps.Samplers.Count = content.Samplers.Count;
            Write(content.Samplers);

            lumps.VertexElements.Offset = Position;
            lumps.VertexElements.Count = content.VertexElements.Count;
            Write(content.VertexElements);

            lumps.BufferViews.Offset = Position;
            lumps.BufferViews.Count = content.BufferViews.Count;
            Write(content.BufferViews);

            lumps.UnlitMaterials.Offset = Position;
            lumps.UnlitMaterials.Count = content.UnlitMaterials.Count;
            Write(content.UnlitMaterials);

            lumps.MetallicRoughnessMaterials.Offset = Position;
            lumps.MetallicRoughnessMaterials.Count = content.MetallicRoughnessMaterials.Count;
            Write(content.MetallicRoughnessMaterials);

            lumps.SpecularGlossinessMaterials.Offset = Position;
            lumps.SpecularGlossinessMaterials.Count = content.SpecularGlossinessMaterials.Count;
            Write(content.SpecularGlossinessMaterials);

            lumps.MaterialBindings.Offset = Position;
            lumps.MaterialBindings.Count = content.MaterialBindings.Count;
            Write(content.MaterialBindings);

            lumps.Primitives.Offset = Position;
            lumps.Primitives.Count = content.Primitive.Count;
            Write(content.Primitive);

            lumps.Meshes.Offset = Position;
            lumps.Meshes.Count = content.Mesh.Count;
            Write(content.Mesh);

            lumps.Nodes.Offset = Position;
            lumps.Nodes.Count = content.Node.Count;
            Write(content.Node);

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

        private void Write<T>(T[] value) where T : struct
        {
            for (var index = 0; index < value.Length; index++) Write(ref value[index]);
        }

        private void Write<T>(IList<T> value) where T : struct
        {
            for (var index = 0; index < value.Count; index++) Write(value[index]);
        }
    }
}