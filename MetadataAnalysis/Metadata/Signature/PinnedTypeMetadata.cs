using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata.Signature
{
    public class PinnedTypeMetadata : SignatureTypeMetadata
    {
        public PinnedTypeMetadata(TypeMetadata underlyingType)
            : base("fixed ", underlyingType)
        {
        }

        internal override TypeMetadata InstantiateGenerics(IImmutableList<TypeMetadata> genericArguments)
        {
            return new PinnedTypeMetadata(UnderlyingType.InstantiateGenerics(genericArguments));
        }
    }
}