using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator
{
    public interface ITypeDefinitionMetadata
    {
        string Name { get; }

        string Namespace { get; }

        string FullName { get; }

        Accessibility Accessibility { get; }

        ITypeDefinitionMetadata BaseType { get; }

        IReadOnlyDictionary<string, IFieldMetadata> Fields { get; }

        IReadOnlyDictionary<string, IReadOnlyCollection<IPropertyMetadata>> Properties { get; }

        IReadOnlyDictionary<string, IReadOnlyCollection<IMethodMetadata>> Methods { get; }

        IReadOnlyDictionary<string, ITypeDefinitionMetadata> NestedTypes { get; }

        IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes { get; }

        IReadOnlyCollection<IGenericParameterMetadata> GenericParameters { get; }

        IAssemblyDefinitionMetadata Assembly { get; }
    }
}