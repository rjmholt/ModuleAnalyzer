using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public class StructMetadata : DefinedTypeMetadata
    {
        public StructMetadata(
            string name,
            string @namespace,
            ProtectionLevel protectionLevel,
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
                TypeKind.Struct,
                protectionLevel,
                LoadedTypes.ValueTypeMetadata,
                declaringType,
                constructors,
                fields,
                properties,
                methods,
                genericParameters,
                customAttributes)
        {
        }

        internal StructMetadata InstantiateGeneric(NameableTypeMetadata parameterType, int index)
        {
            return new StructMetadata(
                Name,
                Namespace,
                ProtectionLevel,
                DeclaringType,
                Constructors,
                Fields,
                Properties,
                Methods,
                InstantiateGenericListAtIndex(parameterType, index),
                CustomAttributes);
        }
    }
}