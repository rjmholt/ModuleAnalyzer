using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public abstract class NameableTypeMetadata : TypeMetadata
    {
        protected NameableTypeMetadata(
            string name,
            string @namespace,
            TypeKind typeKind,
            ProtectionLevel protectionLevel,
            TypeMetadata baseType,
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
        }
    }
}