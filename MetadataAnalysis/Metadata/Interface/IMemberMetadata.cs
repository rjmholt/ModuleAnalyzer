namespace MetadataAnalysis.Metadata.Interface
{
    /// <summary>
    /// Represents the metadata on a member of a type.
    /// </summary>
    public interface IMemberMetadata
    {
        /// <summary>
        /// The name of the member.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The accessibility level of the member.
        /// </summary>
        ProtectionLevel ProtectionLevel { get; }

        /// <summary>
        /// Whether or not the member is static
        /// </summary>
        bool IsStatic { get; }
    }
}