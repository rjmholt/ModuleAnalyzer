using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public abstract class DefinedTypeMetadata : NameableTypeMetadata
    {
        protected DefinedTypeMetadata(
            string name,
            string @namespace,
            TypeKind typeKind,
            ProtectionLevel protectionLevel,
            TypeMetadata baseType,
            DefinedTypeMetadata declaringType,
            IImmutableList<ConstructorMetadata> constructors,
            IImmutableDictionary<string, FieldMetadata> fields,
            IImmutableDictionary<string, PropertyMetadata> properties,
            IImmutableDictionary<string, IImmutableList<MethodMetadata>> methods,
            IImmutableList<GenericParameterMetadata> genericParameters = null,
            IImmutableList<CustomAttributeMetadata> customAttributes = null)
                : base(
                    name,
                    @namespace,
                    typeKind,
                    protectionLevel,
                    baseType,
                    constructors,
                    fields,
                    properties,
                    methods,
                    genericParameters,
                    customAttributes)
        {
            DeclaringType = declaringType;
        }

        public DefinedTypeMetadata DeclaringType { get; }

        public IImmutableDictionary<string, DefinedTypeMetadata> NestedTypes { get; internal set; }
    }
}