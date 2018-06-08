namespace MetadataAnalysis.Metadata.Signature
{
    public class PointerTypeMetadata : SignatureTypeMetadata
    {
        public PointerTypeMetadata(NameableTypeMetadata underlyingType)
            : base("* ", underlyingType)
        {
        }
    }
}