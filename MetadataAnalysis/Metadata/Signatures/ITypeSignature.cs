namespace MetadataAnalysis.Metadata.Signatures
{
    public interface ITypeSignature
    {
        TypeMetadata Type { get; }
    }

    public class ByRefTypeSignature : ITypeSignature
    {
        public ByRefTypeSignature(TypeMetadata typeMetadata)
        {
            Type = typeMetadata;
        }

        public TypeMetadata Type { get; }
    }

    public class OutTypeSignature : ITypeSignature
    {
        public OutTypeSignature(TypeMetadata typeMetadata)
        {
            Type = typeMetadata;
        }

        public TypeMetadata Type { get; }
    }
}