using System;
using System.Collections.Immutable;
using MetadataAnalysis.Metadata;

namespace MetadataAnalysis.Metadata
{
    public class EnumMetadata : DefinedTypeMetadata
    {
        public EnumMetadata(
            string name,
            string @namespace,
            string fullName,
            ProtectionLevel protectionLevel,
            TypeCode underlyingEnumType)
            : base(
                name,
                @namespace,
                fullName,
                TypeKind.Enum,
                protectionLevel)
        {
            BaseType = LoadedTypes.EnumTypeMetadata;

            // TODO: Populate the constructors, fields, properties, methods and custom attributes
            //       with appropriate enum bits
            Constructors = ImmutableArray<ConstructorMetadata>.Empty;
            Fields = ImmutableDictionary<string, FieldMetadata>.Empty;
            Properties = ImmutableDictionary<string, PropertyMetadata>.Empty;
            Methods = ImmutableDictionary<string, IImmutableList<MethodMetadata>>.Empty;
            CustomAttributes = ImmutableArray<CustomAttributeMetadata>.Empty;

            UnderlyingEnumType = underlyingEnumType;
        }

        public IImmutableList<EnumMemberMetadata> Members { get; internal set; }

        public TypeCode UnderlyingEnumType { get; }

        internal override TypeMetadata InstantiateGenerics(IImmutableList<TypeMetadata> genericArguments)
        {
            return this;
        }
    }
}