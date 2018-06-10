using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata.Signature
{
    public class PointerTypeMetadata : SignatureTypeMetadata
    {
        public PointerTypeMetadata(TypeMetadata underlyingType)
            : base("* ", underlyingType)
        {
        }

        internal override TypeMetadata InstantiateGenerics(IImmutableList<TypeMetadata> genericArguments)
        {
            return new PointerTypeMetadata(UnderlyingType.InstantiateGenerics(genericArguments));
        }
    }
}