using System.Collections.Immutable;
using MetadataAnalysis.Metadata;
using MetadataAnalysis.Metadata.Interface;

namespace MetadataAnalysis.Metadata.ILParse
{
    public class ILEnumMetadata : ILTypeMetadata, IEnumMetadata
    {
        public ILEnumMetadata(
            string name,
            string @namespace,
            ProtectionLevel protectionLevel,
            IImmutableList<string> members) :
            base(
                name,
                @namespace,
                TypeKind.Enum,
                protectionLevel,
                LoadedTypes.EnumTypeMetadata,
                ImmutableArray<IConstructorMetadata>.Empty,
                ImmutableDictionary<string, IFieldMetadata>.Empty,
                ImmutableDictionary<string, IPropertyMetadata>.Empty,
                ImmutableDictionary<string, IImmutableList<IMethodMetadata>>.Empty,
                ImmutableDictionary<string, ITypeMetadata>.Empty)
        {
            Members = members;
        }

        public IImmutableList<string> Members { get; }
    }
}