using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Interface;

namespace MetadataAnalysis.Metadata.ILParse
{
    public class ILStructMetadata : ILTypeMetadata, IStructMetadata
    {
        public ILStructMetadata(
            string name,
            string @namespace,
            ProtectionLevel protectionLevel,
            IImmutableList<IConstructorMetadata> constructors,
            IImmutableDictionary<string, IFieldMetadata> fields,
            IImmutableDictionary<string, IPropertyMetadata> properties,
            IImmutableDictionary<string, IImmutableList<IMethodMetadata>> methods,
            IImmutableDictionary<string, ITypeMetadata> nestedTypes)
            : base(
                name,
                @namespace,
                TypeKind.Struct,
                protectionLevel,
                LoadedTypes.ValueTypeMetadata,
                constructors,
                fields,
                properties,
                methods,
                nestedTypes)
        {
        }
    }
}