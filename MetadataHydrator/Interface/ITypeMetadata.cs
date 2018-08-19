using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator
{
    public interface ITypeMetadata
    {
        string Name { get; }

        string Namespace { get; }

        string FullName { get; }

        Accessibility Accessibility { get; }

        ITypeMetadata BaseType { get; }

        IReadOnlyDictionary<string, IFieldMetadata> Fields { get; }

        IReadOnlyDictionary<string, IReadOnlyCollection<IPropertyMetadata>> Properties { get; }

        IReadOnlyDictionary<string, IReadOnlyCollection<IMethodMetadata>> Methods { get; }

        IReadOnlyDictionary<string, ITypeMetadata> NestedTypes { get; }

        IAssemblyMetadata Assembly { get; }
    }
}