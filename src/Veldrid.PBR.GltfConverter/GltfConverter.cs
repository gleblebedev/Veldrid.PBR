using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using SharpGLTF.Schema2;
using SharpGLTF.Validation;
using Veldrid.PBR.BinaryData;
using Veldrid.PBR.DataStructures;

namespace Veldrid.PBR
{
    public class GltfConverter
    {
        public const string TargetPrefix = "TARGET_";
        public MemoryStream _indexBuffer;
        public BinaryWriter _indexWriter;
        public MemoryStream _vertexBuffer;
        private readonly ModelRoot _modelRoot;
        private readonly BinaryWriter _vertexWriter;
        private readonly ContentToWrite _content;
        private int _defaultMaterial = -1;
        private Dictionary<ComparableList<VertexElementData>, IndexRange> _elements = new Dictionary<ComparableList<VertexElementData>, IndexRange>();

        struct MaterialRef
        {
            public int Index;
            public MaterialType Type;
        }

        private List<MaterialRef> _materials;

        internal GltfConverter(ModelRoot modelRoot)
        {
            _modelRoot = modelRoot;
            _content = new ContentToWrite();
            _indexBuffer = new MemoryStream();
            _indexWriter = new BinaryWriter(_indexBuffer);
            _vertexBuffer = new MemoryStream();
            _vertexWriter = new BinaryWriter(_vertexBuffer);
        }

        public byte[] Content { get; private set; }

        public static byte[] ReadGtlf(string fileName)
        {
            var modelRoot = ModelRoot.Load(fileName, new ReadSettings {Validation = ValidationMode.Skip});
            var converter = new GltfConverter(modelRoot);
            converter.Convert();
            return converter.Content;
        }

        private void Convert()
        {
            var skinPerMesh = new Skin[_modelRoot.LogicalMeshes.Count];
            foreach (var node in _modelRoot.LogicalNodes)
                if (node.Skin != null && node.Mesh != null)
                    skinPerMesh[node.Mesh.LogicalIndex] = node.Skin;
            foreach (var texture in _modelRoot.LogicalTextures)
            {
                var textureData = new TextureData(_content.AddBlob(texture.PrimaryImage.GetImageContent()))
                {
                    Name = _content.AddString(texture.PrimaryImage.Name)
                };
                _content.Textures.Add(textureData);
            }
            foreach (var sampler in _modelRoot.LogicalTextureSamplers)
            {
                ConvertSampler(sampler);
            }
            _materials = new List<MaterialRef>(_modelRoot.LogicalMaterials.Count);
            foreach (var material in _modelRoot.LogicalMaterials)
            {
                _materials.Add(CreateUnlitMaterial(material));
            }
            foreach (var mesh in _modelRoot.LogicalMeshes) ConvertMesh(mesh, skinPerMesh[mesh.LogicalIndex]);
            _content.Buffers.Add(new BufferData(_content.AddBlob(_vertexBuffer), BufferUsage.VertexBuffer));
            _content.Buffers.Add(new BufferData(_content.AddBlob(_indexBuffer), BufferUsage.IndexBuffer));

            foreach (var logicalNode in _modelRoot.LogicalNodes) ConvertNode(logicalNode);
            var buffer = new MemoryStream();
            using (var writer = new ContentWriter(buffer, true))
            {
                writer.Write(_content);
            }

            Content = buffer.ToArray();
        }

        private void ConvertSampler(TextureSampler sampler)
        {
            var samplerData = new SamplerData();
            samplerData.AddressModeU = GetAddressMode(sampler.WrapS);
            samplerData.AddressModeV = GetAddressMode(sampler.WrapT);
            samplerData.AddressModeW = SamplerAddressMode.Wrap;
            samplerData.Filter = GetFilter(sampler.MinFilter, sampler.MagFilter);
            samplerData.ComparisonKind = null;
            samplerData.MaximumAnisotropy = 4;
            samplerData.MinimumLod = 0;
            samplerData.MaximumLod = UInt32.MaxValue;
            samplerData.LodBias = 0;
            samplerData.BorderColor = SamplerBorderColor.TransparentBlack;
            _content.Samplers.Add(samplerData);
        }

        private SamplerAddressMode GetAddressMode(TextureWrapMode samplerWrapS)
        {
            switch (samplerWrapS)
            {
                case TextureWrapMode.CLAMP_TO_EDGE:
                    return SamplerAddressMode.Clamp;
                case TextureWrapMode.MIRRORED_REPEAT:
                    return SamplerAddressMode.Mirror;
                case TextureWrapMode.REPEAT:
                    return SamplerAddressMode.Wrap;
                default:
                    throw new ArgumentOutOfRangeException(nameof(samplerWrapS), samplerWrapS, null);
            }
        }

        private SamplerFilter GetFilter(TextureMipMapFilter samplerMinFilter, TextureInterpolationFilter samplerMagFilter)
        {
            return SamplerFilter.Anisotropic;
        }

        private MaterialRef CreateUnlitMaterial(Material material)
        {
            var unlitMaterial = UnlitMaterialData.Default;
            unlitMaterial.Base = CreateMaterialBase(material);
            var baseColor = material.FindChannel(KnownChannels.BaseColor) ?? material.FindChannel(KnownChannels.Diffuse);
            if (baseColor != null)
            {
                var channel = baseColor.Value;
                unlitMaterial.Base.BaseColorFactor = channel.Parameter;
                if (channel.Texture != null)
                    unlitMaterial.BaseColorMap = channel.Texture.LogicalIndex;
                unlitMaterial.BaseColorMapUV = GetUV(channel.TextureCoordinate, channel.TextureTransform);
                if (channel.TextureSampler != null)
                    unlitMaterial.BaseColorSampler = channel.TextureSampler.LogicalIndex;
            }
 
            var res = new MaterialRef() {Index = _content.UnlitMaterials.Count, Type = MaterialType.Unlit};
            _content.UnlitMaterials.Add(unlitMaterial);
            return res;
        }

        private MapUV GetUV(int uvSet, TextureTransform transform)
        {
            var res = MapUV.Default;
            res.Set = (int) (transform?.TextureCoordinateOverride ?? uvSet);
            if (transform != null)
            {
                var num1 = (float)Math.Cos(transform.Rotation);
                var num2 = (float)Math.Sin(transform.Rotation);
                var M11 = num1 * transform.Scale.X;
                var M12 = num2 * transform.Scale.X;
                var M21 = -num2 * transform.Scale.Y;
                var M22 = num1 * transform.Scale.Y;
                var transformOffset = transform.Offset;

                res.X = new Vector3(M11, M12, transformOffset.X);
                res.Y = new Vector3(M21, M22, transformOffset.Y);
            }
            return res;
        }


        private MaterialDataBase CreateMaterialBase(Material material)
        {
            var materialBase = new MaterialDataBase()
            {
                AlphaMode = GetAlphaMode(material.Alpha),
                AlphaCutoff = material.AlphaCutoff,
                BaseColorFactor = Vector4.One,
                FaceCullMode = material.DoubleSided ? FaceCullMode.None : FaceCullMode.Back
            };
            var baseColor = material.FindChannel(KnownChannels.BaseColor) ?? material.FindChannel(KnownChannels.Diffuse);
            if (baseColor != null)
            {
                materialBase.BaseColorFactor = baseColor.Value.Parameter;
            }
            return materialBase;
        }

        public static class KnownChannels
        {
            public const string Normal = "Normal";
            public const string Occlusion = "Occlusion";
            public const string Emissive = "Emissive";

            public const string BaseColor = "BaseColor";
            public const string MetallicRoughness = "MetallicRoughness";

            public const string Diffuse = "Diffuse";
            public const string SpecularGlossiness = "SpecularGlossiness";
        }

        private AlphaMode GetAlphaMode(SharpGLTF.Schema2.AlphaMode materialAlpha)
        {
            switch (materialAlpha)
            {
                case SharpGLTF.Schema2.AlphaMode.OPAQUE:
                    return AlphaMode.Opaque;
                case SharpGLTF.Schema2.AlphaMode.MASK:
                    return AlphaMode.Mask;
                case SharpGLTF.Schema2.AlphaMode.BLEND:
                    return AlphaMode.Blend;
                default:
                    throw new ArgumentOutOfRangeException(nameof(materialAlpha), materialAlpha, null);
            }
        }

        private void ConvertNode(Node logicalNode)
        {
            if (_content.Node.Count != logicalNode.LogicalIndex)
                throw new InvalidDataException("Index mismatch");
            var nodeData = new NodeData
            {
                Name = _content.AddString(logicalNode.Name),
                ParentNode = logicalNode.VisualParent == null ? -1 : logicalNode.VisualParent.LogicalIndex,
                MeshIndex = -1,
                LocalTransform = logicalNode.LocalMatrix,
                WorldTransform = logicalNode.WorldMatrix,
            };
            if (logicalNode.Mesh != null)
            {
                nodeData.MeshIndex = logicalNode.Mesh.LogicalIndex;
                nodeData.MaterialBindings = new IndexRange(_content.MaterialBindings.Count, logicalNode.Mesh.Primitives.Count);
                foreach (var primitive in logicalNode.Mesh.Primitives)
                {
                    if (primitive.Material == null)
                    {
                        if (_defaultMaterial < 0)
                        {
                            _defaultMaterial = _content.UnlitMaterials.Count;
                            _content.UnlitMaterials.Add(UnlitMaterialData.Default);
                        }
                        _content.MaterialBindings.Add(new MaterialReference(MaterialType.Unlit, _defaultMaterial));
                    }
                    else
                    {
                        _content.MaterialBindings.Add(new MaterialReference(MaterialType.Unlit, primitive.Material.LogicalIndex));
                    }
                }
            }
            _content.Node.Add(nodeData);
        }

        private void ConvertMesh(Mesh mesh, Skin skin)
        {
            var meshData = new MeshData(new IndexRange(_content.Primitive.Count, mesh.Primitives.Count));
            meshData.Name = _content.AddString(mesh.Name);
            foreach (var primitive in mesh.Primitives)
            {
                var primitiveData = new PrimitiveData();
                primitiveData.IndexBuffer = 1;
                primitiveData.IndexBufferOffset = (uint) _indexBuffer.Position;
                primitiveData.VertexBufferView = _content.BufferViews.Count;
                List<int> indices;
                switch (primitive.DrawPrimitiveType)
                {
                    case PrimitiveType.POINTS:
                        indices = primitive.GetPointIndices().ToList();
                        primitiveData.PrimitiveTopology = PrimitiveTopology.PointList;
                        break;
                    case PrimitiveType.LINES:
                        indices = GetLineIndices(primitive).ToList();
                        primitiveData.PrimitiveTopology = PrimitiveTopology.LineList;
                        break;
                    case PrimitiveType.LINE_LOOP:
                        indices = GetLineIndices(primitive).ToList();
                        primitiveData.PrimitiveTopology = PrimitiveTopology.LineList;
                        break;
                    case PrimitiveType.LINE_STRIP:
                        indices = GetLineIndices(primitive).ToList();
                        primitiveData.PrimitiveTopology = PrimitiveTopology.LineList;
                        break;
                    case PrimitiveType.TRIANGLES:
                        indices = GetTriangleIndices(primitive).ToList();
                        primitiveData.PrimitiveTopology = PrimitiveTopology.TriangleList;
                        break;
                    case PrimitiveType.TRIANGLE_STRIP:
                        indices = GetTriangleIndices(primitive).ToList();
                        primitiveData.PrimitiveTopology = PrimitiveTopology.TriangleList;
                        break;
                    case PrimitiveType.TRIANGLE_FAN:
                        indices = GetTriangleIndices(primitive).ToList();
                        primitiveData.PrimitiveTopology = PrimitiveTopology.TriangleList;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var attributes = GetVertexAttributes(primitive);
                var pos = attributes.Where(_ => _.Key == "POSITION").FirstOrDefault() as Float3VertexAttribute;
                if (pos != null)
                {
                    var bboxMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                    var bboxMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                    foreach (var index in indices)
                    {
                        var p = pos.Values[index];
                        bboxMin = Vector3.Min(p, bboxMin);
                        bboxMax = Vector3.Max(p, bboxMax);
                    }

                    primitiveData.BoundingBoxMin = bboxMin;
                    primitiveData.BoundingBoxMax = bboxMax;
                    primitiveData.SphereCenter = (bboxMin + bboxMax) * 0.5f;
                    primitiveData.SphereRadius = ((bboxMin - bboxMax) * 0.5f).Length(); 
                }
                var newIndices = new Dictionary<int, int>();
                primitiveData.IndexBufferFormat =
                    indices.Count <= ushort.MaxValue ? IndexFormat.UInt16 : IndexFormat.UInt32;
                foreach (var index in indices)
                {
                    int v;
                    if (!newIndices.TryGetValue(index, out v))
                    {
                        v = newIndices.Count;
                        newIndices.Add(index, v);
                        foreach (var attribute in attributes) attribute.Write(_vertexWriter, index);
                    }

                    if (primitiveData.IndexBufferFormat == IndexFormat.UInt16)
                        _indexWriter.Write((ushort) v);
                    else
                        _indexWriter.Write((uint) v);
                }

                primitiveData.IndexCount = (uint) indices.Count;

                _content.Primitive.Add(primitiveData);
            }

            _content.Mesh.Add(meshData);
        }

        private List<AbstractVertexAttribute> GetVertexAttributes(MeshPrimitive primitive)
        {
            var vertexAccessors = primitive.VertexAccessors;
            var attributes = new List<AbstractVertexAttribute>(vertexAccessors.Count);
            AbstractVertexAttribute positions = null;
            AbstractVertexAttribute normals = null;
            AbstractVertexAttribute colors = null;
            foreach (var vertexAccessor in vertexAccessors)
            {
                var key = vertexAccessor.Key;
                var accessor = vertexAccessor.Value;
                var attribute = AbstractVertexAttribute.Create(key, accessor);
                attributes.Add(attribute);
                switch (key)
                {
                    case "POSITION":
                        positions = attribute;
                        break;
                    case "NORMAL":
                        normals = attribute;
                        break;
                    case "COLOR_0":
                        colors = attribute;
                        break;
                }
            }

            if (normals == null)
            {
                normals = GenerateNormals((Float3VertexAttribute)positions, primitive);
                attributes.Add(normals);
            }
            //if (colors == null)
            //{
            //    colors = GenerateColors();
            //    attributes.Add(colors);
            //}


            for (int morphTargetIndex = 0; morphTargetIndex < primitive.MorphTargetsCount; ++morphTargetIndex)
            {
                foreach (var morphTargetAccessor in primitive.GetMorphTargetAccessors(morphTargetIndex))
                {
                    var key = TargetPrefix+morphTargetAccessor.Key+"_"+morphTargetIndex;
                    var accessor = morphTargetAccessor.Value;
                    var attribute = AbstractVertexAttribute.Create(key, accessor);
                    attributes.Add(attribute);
                }
            }
            attributes.Sort(new VertexAttributeComparer());
            var elements = new ComparableList<VertexElementData>(attributes.Select(_=> new VertexElementData(_content.AddString(_.Key), _.VertexElementFormat, VertexElementSemantic.TextureCoordinate)));


            if (!_elements.TryGetValue(elements, out var range))
            {
                range = new IndexRange(_content.VertexElements.Count, elements.Count);
                foreach (var element in elements)
                {
                    _content.VertexElements.Add(element);
                }
            }


            var vertexBufferViewData = new VertexBufferViewData(0, (uint) _vertexBuffer.Position, range);

            _content.BufferViews.Add(vertexBufferViewData);

            return attributes;
        }

        private Vector3ArrayVertexAttribute GenerateNormals(Float3VertexAttribute positions, MeshPrimitive primitive)
        {
            var normals = new Vector3[positions.Values.Count];
            if (primitive.DrawPrimitiveType == PrimitiveType.TRIANGLES
                || primitive.DrawPrimitiveType == PrimitiveType.TRIANGLE_FAN
                || primitive.DrawPrimitiveType == PrimitiveType.TRIANGLE_STRIP)
            {
                foreach ((int A, int B, int C) in primitive.GetTriangleIndices())
                {
                    var ab = positions.Values[B] - positions.Values[A];
                    var ac = positions.Values[C] - positions.Values[A];
                    var n = Vector3.Cross(ab, ac);
                    normals[A] += n;
                    normals[B] += n;
                    normals[C] += n;
                }
            }

            for (var index = 0; index < normals.Length; index++)
            {
                var normal = normals[index];
                if (normal != Vector3.Zero)
                    normals[index] = Vector3.Normalize(normal);
                else
                    normals[index] = Vector3.UnitY;
            }

            return new Vector3ArrayVertexAttribute("NORMAL", normals);
        }

        private IEnumerable<int> GetTriangleIndices(MeshPrimitive primitive)
        {
            foreach (var valueTuple in primitive.GetTriangleIndices())
            {
                yield return valueTuple.C;
                yield return valueTuple.B;
                yield return valueTuple.A;
            }
        }

        private IEnumerable<int> GetLineIndices(MeshPrimitive primitive)
        {
            foreach (var valueTuple in primitive.GetLineIndices())
            {
                yield return valueTuple.A;
                yield return valueTuple.B;
            }
        }
    }
}