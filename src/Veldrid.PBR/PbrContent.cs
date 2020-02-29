using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid.ImageSharp;
using Veldrid.PBR.BinaryData;
using Veldrid.PBR.Uniforms;

namespace Veldrid.PBR
{
    public class PbrContent : IDisposable
    {
        internal static readonly VersionValue CurrentVersion = new VersionValue(new Version(0, 0, 0, 0));

        internal static readonly byte[] Magic =
            Encoding.ASCII.GetBytes("VELDRIDPBR" + (BitConverter.IsLittleEndian ? "LE" : "BE"));

        private readonly Memory<byte> _data;
        private readonly Sampler[] _samplers;
        private readonly Texture[] _textures;
        private readonly TextureView[] _textureViews;
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

            _samplers = new Sampler[NumSamplers];
            _textures = new Texture[NumTextures];
            _textureViews = new TextureView[NumTextures];
        }

        public int NumBuffers => _chunks.Buffers.Count;
        public int NumSamplers => _chunks.Samplers.Count;
        public int NumTextures => _chunks.Textures.Count;
        public int NumUnlitMaterials => _chunks.UnlitMaterials.Count;
        public int NumMetallicRoughnessMaterials => _chunks.MetallicRoughnessMaterials.Count;
        public int NumSpecularGlossinessMaterials => _chunks.SpecularGlossinessMaterials.Count;
        public int NumMeshes => _chunks.Meshes.Count;
        public int NumNodes => _chunks.Nodes.Count;

        public Texture GetOrCreateTexture(uint index, GraphicsDevice graphicsDevice, ResourceFactory resourceFactory)
        {
            if (index < 0)
                return null;
            return _textures[index] ?? (_textures[index] = CreateTexture(index, graphicsDevice, resourceFactory));
        }

        public Texture CreateTexture(uint index, GraphicsDevice graphicsDevice, ResourceFactory resourceFactory)
        {
            var textureData = _chunks.Textures.GetAt(_data, index);
            var textureBlob = new MemoryStream(_data.Span.Slice(GetBlob(textureData.BlobIndex)).ToArray());

            var imageSharpTexture = new ImageSharpTexture(textureBlob);
            return imageSharpTexture.CreateDeviceTexture(graphicsDevice, resourceFactory);
        }

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

        public void CreateSamplerDescription(uint index, out SamplerDescription description)
        {
            ref var samplerData = ref _chunks.Samplers.GetAt(_data, index);
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

        public Sampler GetOrCreateSampler(IdRef<SamplerData> id, GraphicsDevice graphicsDevice, ResourceFactory resourceFactory)
        {
            if (!id.HasValue)
                return graphicsDevice.Aniso4xSampler;
            var index = id.Value;
            return _samplers[index] ?? (_samplers[index] = CreateSampler(index, resourceFactory));
        }

        public Sampler CreateSampler(uint index, ResourceFactory resourceFactory)
        {
            CreateSamplerDescription(index, out var description);
            return resourceFactory.CreateSampler(description);
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
        public ref MaterialReference GetMaterialReference(int index)
        {
            return ref _chunks.MaterialBindings.GetAt(_data, index);
        }

        public UnlitMaterial CreateUnlitMaterial(int index, GraphicsDevice graphicsDevice,
            ResourceFactory resourceFactory)
        {
            ref var unlitMaterialData = ref _chunks.UnlitMaterials.GetAt(_data, index);
            return new UnlitMaterial
            {
                AlphaMode = unlitMaterialData.AlphaMode,
                AlphaCutoff = unlitMaterialData.UniformArguments.AlphaCutoff,
                BaseColorFactor = unlitMaterialData.UniformArguments.BaseColorFactor,
                FaceCullMode = unlitMaterialData.FaceCullMode,
                BaseColorMap = GetMapParameters(graphicsDevice, resourceFactory, unlitMaterialData.BaseColor, ref unlitMaterialData.UniformArguments.BaseColorMapUV)
            };
        }

        public UnlitMaterial CreateMetallicRoughnessMaterial(int index, GraphicsDevice graphicsDevice,
            ResourceFactory resourceFactory)
        {
            ref var unlitMaterialData = ref _chunks.MetallicRoughnessMaterials.GetAt(_data, index);
            return new UnlitMaterial
            {
                AlphaMode = unlitMaterialData.AlphaMode,
                AlphaCutoff = unlitMaterialData.UniformArguments.AlphaCutoff,
                BaseColorFactor = unlitMaterialData.UniformArguments.BaseColorFactor,
                FaceCullMode = unlitMaterialData.FaceCullMode,
                BaseColorMap = GetMapParameters(graphicsDevice, resourceFactory, unlitMaterialData.BaseColor, ref unlitMaterialData.UniformArguments.BaseColorMapUV)
            };
        }

        public UnlitMaterial CreateSpecularGlossinessMaterial(int index, GraphicsDevice graphicsDevice,
            ResourceFactory resourceFactory)
        {
            ref var unlitMaterialData = ref _chunks.MetallicRoughnessMaterials.GetAt(_data, index);
            return new UnlitMaterial
            {
                AlphaMode = unlitMaterialData.AlphaMode,
                AlphaCutoff = unlitMaterialData.UniformArguments.AlphaCutoff,
                BaseColorFactor = unlitMaterialData.UniformArguments.BaseColorFactor,
                FaceCullMode = unlitMaterialData.FaceCullMode,
                BaseColorMap = GetMapParameters(graphicsDevice, resourceFactory, unlitMaterialData.BaseColor, ref unlitMaterialData.UniformArguments.BaseColorMapUV)
            };
        }
        
        private MapParameters GetMapParameters(GraphicsDevice graphicsDevice, ResourceFactory resourceFactory,
            MapAndSampler mapAndSampler, ref MapUV mapUv)
        {
            return new MapParameters
            {
                Sampler = GetOrCreateSampler(mapAndSampler.Sampler, graphicsDevice, resourceFactory),
                Map = GetOrCreateTextureView(mapAndSampler.Map, graphicsDevice, resourceFactory),
                UV = mapUv
            };
        }

        public void Dispose()
        {
            foreach (var sampler in _samplers) sampler?.Dispose();
            foreach (var textureView in _textureViews) textureView?.Dispose();
            foreach (var texture in _textures) texture?.Dispose();
        }

        private void GetBufferDescription(int index, out BufferDescription description)
        {
            ref var bufferData = ref _chunks.Buffers.GetAt(_data, index);
            ref var blob = ref _chunks.BinaryBlobs.GetAt(_data, bufferData.BlobIndex);
            description.SizeInBytes = (uint) blob.Count;
            description.Usage = bufferData.Usage;
            description.StructureByteStride = bufferData.StructureByteStride;
            description.RawBuffer = bufferData.RawBuffer;
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

        private TextureView GetOrCreateTextureView(IdRef<TextureData> id, GraphicsDevice graphicsDevice,
            ResourceFactory resourceFactory)
        {
            if (!id.HasValue)
                return null;
            var index = id.Value;
            return _textureViews[index] ?? (_textureViews[index] =
                       resourceFactory.CreateTextureView(
                           new TextureViewDescription(GetOrCreateTexture(index, graphicsDevice, resourceFactory))));
        }
    }
}