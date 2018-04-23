namespace MetadataAnalysis.Metadata.Interface
{
    public interface IMemberMetadata
    {
        string Name { get; }

         ProtectionLevel ProtectionLevel { get; }

         bool IsStatic { get; }
    }
}