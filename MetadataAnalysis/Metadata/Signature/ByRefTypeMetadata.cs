using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata.Signature
{
    public class OutTypeMetadata : SignatureTypeMetadata
    {
        public OutTypeMetadata(NameableTypeMetadata underlyingType)
            : base("out ", underlyingType)
        {
        }
    }

    public class ByRefTypeMetadata : SignatureTypeMetadata
    {
        public ByRefTypeMetadata(NameableTypeMetadata underlyingType)
            : base("ref ",underlyingType)
        {
        }
    }
}