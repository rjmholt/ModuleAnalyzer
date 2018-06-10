using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata.Signature
{
    public class VolatileTypeMetadata : SignatureTypeMetadata
    {
        public VolatileTypeMetadata(TypeMetadata underlyingType)
            : base("volatile ", underlyingType)
        {
        }

        internal override TypeMetadata InstantiateGenerics(IImmutableList<TypeMetadata> genericArguments)
        {
            return new VolatileTypeMetadata(UnderlyingType.InstantiateGenerics(genericArguments));
        }
    }
}