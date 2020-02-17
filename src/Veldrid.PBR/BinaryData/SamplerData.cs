namespace Veldrid.PBR.BinaryData
{
    public struct SamplerData
    {
        /// <summary>
        ///     The <see cref="SamplerAddressMode" /> mode to use for the U (or S) coordinate.
        /// </summary>
        public SamplerAddressMode AddressModeU;

        /// <summary>
        ///     The <see cref="SamplerAddressMode" /> mode to use for the V (or T) coordinate.
        /// </summary>
        public SamplerAddressMode AddressModeV;

        /// <summary>
        ///     The <see cref="SamplerAddressMode" /> mode to use for the W (or R) coordinate.
        /// </summary>
        public SamplerAddressMode AddressModeW;

        /// <summary>
        ///     The filter used when sampling.
        /// </summary>
        public SamplerFilter Filter;

        /// <summary>
        ///     An optional value controlling the kind of comparison to use when sampling. If null, comparison sampling is not
        ///     used.
        /// </summary>
        public ComparisonKind? ComparisonKind;

        /// <summary>
        ///     The maximum anisotropy of the filter, when <see cref="SamplerFilter.Anisotropic" /> is used, or otherwise ignored.
        /// </summary>
        public uint MaximumAnisotropy;

        /// <summary>
        ///     The minimum level of detail.
        /// </summary>
        public uint MinimumLod;

        /// <summary>
        ///     The maximum level of detail.
        /// </summary>
        public uint MaximumLod;

        /// <summary>
        ///     The level of detail bias.
        /// </summary>
        public int LodBias;

        /// <summary>
        ///     The constant color that is sampled when <see cref="SamplerAddressMode.Border" /> is used, or otherwise ignored.
        /// </summary>
        public SamplerBorderColor BorderColor;
    }
}