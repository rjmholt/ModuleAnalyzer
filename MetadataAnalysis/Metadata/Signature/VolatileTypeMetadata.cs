namespace MetadataAnalysis.Metadata.Signature
{
    public class VolatileTypeMetadata : SignatureTypeMetadata
    {
        public VolatileTypeMetadata(NameableTypeMetadata underlyingType)
            : base("volatile ", underlyingType)
        {
        }
    }
}