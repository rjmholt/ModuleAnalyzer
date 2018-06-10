using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata.Signature
{
    public class OutTypeMetadata : SignatureTypeMetadata
    {
        public OutTypeMetadata(TypeMetadata underlyingType)
            : base("out ", underlyingType)
        {
        }

        internal override TypeMetadata InstantiateGenerics(IImmutableList<TypeMetadata> genericArguments)
        {
            return new OutTypeMetadata(UnderlyingType.InstantiateGenerics(genericArguments));
        }
    }

    public class ByRefTypeMetadata : SignatureTypeMetadata
    {
        public ByRefTypeMetadata(TypeMetadata underlyingType)
            : base("ref ",underlyingType)
        {
        }

        internal override TypeMetadata InstantiateGenerics(IImmutableList<TypeMetadata> genericArguments)
        {
            return new ByRefTypeMetadata(UnderlyingType.InstantiateGenerics(genericArguments));
        }
    }
}