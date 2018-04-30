namespace MetadataAnalysis.Metadata
{
    /// <summary>
    /// The protection or accessibility level of a type or member.
    /// </summary>
    public enum ProtectionLevel
    {
        // TODO: Encode protected and/or internal
        Private,
        Protected,
        Internal,
        Public
    }
}