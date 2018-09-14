using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator
{
    public interface IMethodMetadata : IMemberMetadata
    {
        ITypeDefinitionMetadata ReturnType { get; }

        Accessibility Accessibility { get; }

        bool IsStatic { get; }

        bool isAbstract { get; }

        IReadOnlyCollection<ITypeDefinitionMetadata> ParameterTypes { get; }

        IReadOnlyCollection<IGenericParameterMetadata> GenericParameters { get; }
    }
}