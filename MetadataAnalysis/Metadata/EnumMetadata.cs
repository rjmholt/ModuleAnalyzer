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
                declaringType)
        {
            // TODO: Populate the constructors, fields, properties, methods and custom attributes
            //       with appropriate enum bits
            Constructors = ImmutableArray<ConstructorMetadata>.Empty;
            Fields = ImmutableDictionary<string, FieldMetadata>.Empty;
            Properties = ImmutableDictionary<string, PropertyMetadata>.Empty;
            Methods = ImmutableDictionary<string, IImmutableList<MethodMetadata>>.Empty;
            CustomAttributes = ImmutableArray<CustomAttributeMetadata>.Empty;

            UnderlyingEnumType = underlyingEnumType;
            Members = members;
        }

        public IImmutableList<EnumMemberMetadata> Members { get; }

        public PrimitiveTypeCode UnderlyingEnumType { get; }

        internal override TypeMetadata InstantiateGenerics(IImmutableList<TypeMetadata> genericArguments)
        {
            return this;
        }
    }
}