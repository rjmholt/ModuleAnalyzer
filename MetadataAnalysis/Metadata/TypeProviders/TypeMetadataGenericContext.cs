using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata.TypeProviders
{
    public class TypeMetadataGenericContext
    {
        public TypeMetadataGenericContext(
            ImmutableArray<TypeMetadata> typeParameters,
            ImmutableArray<TypeMetadata> methodParameters)
        {
            TypeParameters = typeParameters;
            MethodParameters = methodParameters;
        }

        public ImmutableArray<TypeMetadata> TypeParameters { get; }

        public ImmutableArray<TypeMetadata> MethodParameters { get; }
    }
}