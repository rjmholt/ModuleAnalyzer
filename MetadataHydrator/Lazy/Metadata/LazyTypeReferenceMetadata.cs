using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator.Lazy.Metadata
{
    internal class LazyTypeReferenceMetadata : ITypeMetadata
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

        public Accessibility Accessibility => throw new System.NotImplementedException();

        public ITypeMetadata BaseType => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, IFieldMetadata> Fields => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, IReadOnlyCollection<IPropertyMetadata>> Properties => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, IReadOnlyCollection<IMethodMetadata>> Methods => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, ITypeMetadata> NestedTypes => throw new System.NotImplementedException();

        public IAssemblyMetadata Assembly => throw new System.NotImplementedException();
    }
}