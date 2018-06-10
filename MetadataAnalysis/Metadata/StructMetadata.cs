using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public class StructMetadata : DefinedTypeMetadata
    {
        public new class Prototype : DefinedTypeMetadata.Prototype, IPrototype<StructMetadata>
        {
            protected Prototype(string name, string @namespace, TypeKind typeKind, ProtectionLevel protectionLevel)
                : base(name, @namespace, typeKind, protectionLevel)
            {
            }
        }

        public StructMetadata(
            string name,
            string @namespace,
            ProtectionLevel protectionLevel,
            DefinedTypeMetadata declaringType)
            : base(
                name,
                @namespace,
                TypeKind.Struct,
                protectionLevel,
                LoadedTypes.ValueTypeMetadata,
                declaringType)
        {
        }

        internal override TypeMetadata InstantiateGenerics(IImmutableList<TypeMetadata> genericArguments)
        {
            return new StructMetadata(
                Name,
                Namespace,
                ProtectionLevel,
                DeclaringType)
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