using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator
{
    public interface ITypeDefinitionMetadata : ITypeReferenceMetadata
    {
        Accessibility Accessibility { get; }

        ITypeReferenceMetadata BaseType { get; }

        IReadOnlyDictionary<string, IFieldMetadata> Fields { get; }

        IReadOnlyDictionary<string, IReadOnlyCollection<IPropertyMetadata>> Properties { get; }

        IReadOnlyDictionary<string, IReadOnlyCollection<IMethodMetadata>> Methods { get; }

        IReadOnlyDictionary<string, ITypeDefinitionMetadata> NestedTypes { get; }

        IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes { get; }

        IReadOnlyCollection<IGenericParameterMetadata> GenericParameters { get; }

        IAssemblyDefinitionMetadata Assembly { get; }
    }
}