using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator
{
    public interface IMethodMetadata : IMemberMetadata
    {
        ITypeReferenceMetadata ReturnType { get; }

        Accessibility Accessibility { get; }

        bool IsStatic { get; }

        bool isAbstract { get; }

        IReadOnlyCollection<ITypeReferenceMetadata> ParameterTypes { get; }

        IReadOnlyCollection<IGenericParameterMetadata> GenericParameters { get; }
    }
}