using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator
{
    public interface IMethodMetadata : IMemberMetadata
    {
        ITypeMetadata ReturnType { get; }

        Accessibility Accessibility { get; }

        bool IsStatic { get; }

        bool isAbstract { get; }

        IReadOnlyCollection<ITypeMetadata> ParameterTypes { get; }

        IReadOnlyCollection<IGenericParameterMetadata> GenericParameters { get; }
    }
}