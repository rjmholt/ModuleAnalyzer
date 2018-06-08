namespace MetadataAnalysis.Metadata.Signature
{
    public class PinnedTypeMetadata : SignatureTypeMetadata
    {
        public PinnedTypeMetadata(NameableTypeMetadata underlyingType)
            : base("fixed ", underlyingType)
        {
        }
    }
}