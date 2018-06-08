using System.Collections.Immutable;
using System.Reflection.Metadata;
using MetadataAnalysis.Metadata;

namespace MetadataAnalysis.Metadata
{
    public class EnumMetadata : DefinedTypeMetadata
    {
        public EnumMetadata(
            string name,
            string @namespace,
            ProtectionLevel protectionLevel,
            DefinedTypeMetadata declaringType,
            PrimitiveTypeCode underlyingEnumType,
            IImmutableList<EnumMemberMetadata> members,
            IImmutableList<CustomAttributeMetadata> customAttributes = null) :
            base(
                name,
                @namespace,
                TypeKind.Enum,
                protectionLevel,
                LoadedTypes.EnumTypeMetadata,
                declaringType,
                ImmutableArray<ConstructorMetadata>.Empty,
                ImmutableDictionary<string, FieldMetadata>.Empty,
                ImmutableDictionary<string, PropertyMetadata>.Empty,
                ImmutableDictionary<string, IImmutableList<MethodMetadata>>.Empty,
                customAttributes: customAttributes)
        {
            UnderlyingEnumType = underlyingEnumType;
            Members = members;
        }

        public IImmutableList<EnumMemberMetadata> Members { get; }

        public PrimitiveTypeCode UnderlyingEnumType { get; }
    }
}