namespace MetadataAnalysis.Metadata.Interface
{
    /// <summary>
    /// Represents the metadata of a class type.
    /// </summary>
    public interface IClassMetadata : ITypeMetadata
    {
        /// <summary>
        /// Whether the class is static or not.
        /// </summary>
        bool IsStatic { get; }

        /// <summary>
        /// Whether the class is abstract or not.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        /// Whether the class is sealed or not.
        /// </summary>
        /// <returns></returns>
        bool IsSealed { get; }
    }
}