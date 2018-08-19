using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace MetadataHydrator.Lazy.Metadata
{
    internal class LazyAssemblyReferenceMetadata : IAssemblyMetadata
    {
        private readonly AssemblyReference _assemblyReference;

        private readonly LazyAssemblyHydrator _assemblyHydrator;

        public LazyAssemblyReferenceMetadata(
            string assemblyName,
            AssemblyReference assemblyReference,
            LazyAssemblyHydrator assemblyHydrator)
        {
            Name = assemblyName;
            _assemblyReference = assemblyReference;
            _assemblyHydrator = assemblyHydrator;
        }

        public string Culture => throw new NotImplementedException();

        public AssemblyFlags Flags => throw new NotImplementedException();

        public AssemblyHashAlgorithm HashAlgorithm => throw new NotImplementedException();

        public string Name { get; }

        public IReadOnlyCollection<byte> PublicKey => throw new NotImplementedException();

        public Version Version => throw new NotImplementedException();

        public IReadOnlyDictionary<string, IAssemblyMetadata> RequiredAssemblies => throw new NotImplementedException();

        public string Path => throw new NotImplementedException();

        public IReadOnlyDictionary<string, ITypeMetadata> DefinedTypes => throw new NotImplementedException();

        public IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes => throw new NotImplementedException();
    }
}