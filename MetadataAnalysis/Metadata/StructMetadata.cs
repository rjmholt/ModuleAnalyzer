using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public class StructMetadata : DefinedTypeMetadata
    {
        public StructMetadata(
            string name,
            string @namespace,
            string fullName,
            ProtectionLevel protectionLevel)
            : base(
                name,
                @namespace,
                fullName,
                TypeKind.Struct,
                protectionLevel)
        {
            BaseType = LoadedTypes.ValueTypeMetadata;
        }

        internal override TypeMetadata InstantiateGenerics(IImmutableList<TypeMetadata> genericArguments)
        {
            return new StructMetadata(
                Name,
                Namespace,
                FullName,
                ProtectionLevel)
            {
                Constructors = Constructors,
                Fields = Fields,
                Properties = Properties,
                Methods = Methods,
                GenericParameters = InstantiateGenericList(genericArguments),
                CustomAttributes = CustomAttributes
            };
        }
    }
}