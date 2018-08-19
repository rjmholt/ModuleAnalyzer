using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator.Lazy.Metadata
{
    internal class LazyTypeMetadata : ITypeMetadata
    {
        private readonly TypeDefinition _typeDefinition;

        private readonly LazyAssemblyHydrator _assemblyHydrator;

        public LazyTypeMetadata(TypeDefinition typeDefintion, LazyAssemblyHydrator assemblyHydrator)
        {
            _typeDefinition = typeDefintion;
            _assemblyHydrator = assemblyHydrator;
        }

        public string Name => throw new System.NotImplementedException();

        public string Namespace => throw new System.NotImplementedException();

        public string FullName => throw new System.NotImplementedException();

        public Accessibility Accessibility => throw new System.NotImplementedException();

        public ITypeMetadata BaseType => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, IFieldMetadata> Fields => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, IReadOnlyCollection<IPropertyMetadata>> Properties => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, IReadOnlyCollection<IMethodMetadata>> Methods => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, ITypeMetadata> NestedTypes => throw new System.NotImplementedException();

        public IAssemblyMetadata Assembly => throw new System.NotImplementedException();
    }
}