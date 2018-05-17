using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata
{
    public class StructMetadata : TypeMetadata
    {
        public StructMetadata(
            string name,
            string @namespace,
            ProtectionLevel protectionLevel,
            TypeMetadata declaringType,
            IImmutableList<ConstructorMetadata> constructors,
            IImmutableDictionary<string, FieldMetadata> fields,
            IImmutableDictionary<string, PropertyMetadata> properties,
            IImmutableDictionary<string, IImmutableList<MethodMetadata>> methods)
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
                methods)
        {
        }
    }
}