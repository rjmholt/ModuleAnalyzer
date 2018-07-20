using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata.TypeProviders
{
    public class TypeMetadataGenericContext
    {
        public TypeMetadataGenericContext(
            IImmutableList<GenericParameterMetadata> typeParameters,
            IImmutableList<GenericParameterMetadata> methodParameters)
        {
            TypeParameters = typeParameters;
            MethodParameters = methodParameters;
        }

        public IImmutableList<GenericParameterMetadata> TypeParameters { get; }

        public IImmutableList<GenericParameterMetadata> MethodParameters { get; }
    }
}