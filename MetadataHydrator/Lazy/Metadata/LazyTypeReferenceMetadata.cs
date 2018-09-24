using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator.Lazy.Metadata
{
    internal class LazyTypeReferenceMetadata : ITypeDefinitionMetadata
    {
        public LazyTypeReferenceMetadata(
            string name,
            string @namespace,
            string fullName)
        {
            Name = name;
            Namespace = @namespace;
            FullName = fullName;
        }

        public string Name { get; }

        public string Namespace { get; }

        public string FullName { get; }

        Accessibility ITypeDefinitionMetadata.Accessibility => throw new System.NotImplementedException();

        ITypeReferenceMetadata ITypeDefinitionMetadata.BaseType => throw new System.NotImplementedException();

        IReadOnlyDictionary<string, IFieldMetadata> ITypeDefinitionMetadata.Fields => throw new System.NotImplementedException();

        IReadOnlyDictionary<string, IReadOnlyCollection<IPropertyMetadata>> ITypeDefinitionMetadata.Properties => throw new System.NotImplementedException();

        IReadOnlyDictionary<string, IReadOnlyCollection<IMethodMetadata>> ITypeDefinitionMetadata.Methods => throw new System.NotImplementedException();

        IReadOnlyDictionary<string, ITypeDefinitionMetadata> ITypeDefinitionMetadata.NestedTypes => throw new System.NotImplementedException();

        IReadOnlyCollection<ICustomAttributeMetadata> ITypeDefinitionMetadata.CustomAttributes => throw new System.NotImplementedException();

        IReadOnlyCollection<IGenericParameterMetadata> ITypeDefinitionMetadata.GenericParameters => throw new System.NotImplementedException();

        IAssemblyDefinitionMetadata ITypeDefinitionMetadata.Assembly => throw new System.NotImplementedException();
    }
}