namespace Veldrid.PBR
{
    /// <summary>
    ///     Describes a single element of a vertex.
    /// </summary>
    public struct VertexElementData
    {
        /// <summary>
        ///     The name of the element.
        /// </summary>
        public int Name;

        /// <summary>
        ///     The semantic type of the element.
        ///     NOTE: When using Veldrid.SPIRV, all vertex elements will use
        ///     <see cref="VertexElementSemantic.TextureCoordinate" />.
        /// </summary>
        public VertexElementSemantic Semantic;

        /// <summary>
        ///     The format of the element.
        /// </summary>
        public VertexElementFormat Format;

        /// <summary>
        ///     The offset in bytes from the beginning of the vertex.
        /// </summary>
        public uint Offset;

        /// <summary>
        ///     Constructs a new VertexElementDescription.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <param name="semantic">The semantic type of the element.</param>
        /// <param name="format">The format of the element.</param>
        public VertexElementData(
            int name,
            VertexElementFormat format,
            VertexElementSemantic semantic)
        {
            Name = name;
            Format = format;
            Semantic = semantic;
            Offset = 0;
        }
    }
}