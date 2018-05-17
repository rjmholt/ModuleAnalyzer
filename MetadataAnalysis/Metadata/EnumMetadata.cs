using System.Collections.Immutable;
using MetadataAnalysis.Metadata;

namespace MetadataAnalysis.Metadata
{
    public class EnumMetadata : TypeMetadata
    {
        public EnumMetadata(
            string name,
            string @namespace,
            ProtectionLevel protectionLevel,
            TypeMetadata declaringType,
            IImmutableList<string> members) :
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
                ImmutableDictionary<string, IImmutableList<MethodMetadata>>.Empty)
        {
            Members = members;
        }

        public IImmutableList<string> Members { get; }
    }
}