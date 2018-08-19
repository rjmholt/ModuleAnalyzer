using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator
{
    public interface IMemberMetadata
    {
        string Name { get; }

        Accessibility Accessibility { get; }

        IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes { get; }

        IReadOnlyCollection<IGenericParameterMetadata> GenericParameters { get; }

        bool IsStatic { get; }
    }
}