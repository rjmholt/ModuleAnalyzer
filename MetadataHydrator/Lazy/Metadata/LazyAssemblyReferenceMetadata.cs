using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;

namespace MetadataHydrator.Lazy.Metadata
{
    internal class LazyAssemblyReferenceMetadata : IAssemblyMetadata
    {
        private readonly AssemblyReference _assemblyReference;

        private readonly LazyAssemblyHydrator _assemblyHydrator;

        private readonly Lazy<string> _culture;

        public LazyAssemblyReferenceMetadata(
            string assemblyName,
            AssemblyReference assemblyReference,
            LazyAssemblyHydrator assemblyHydrator)
        {
            _assemblyReference = assemblyReference;
            _assemblyHydrator = assemblyHydrator;
            Name = assemblyName;
            _culture = new Lazy<string>(() => _assemblyHydrator.ReadString(_assemblyReference.Culture));
        }

        public string Culture
        {
            get => _culture.Value;
        }

        public AssemblyFlags Flags => _assemblyReference.Flags;

        public AssemblyHashAlgorithm HashAlgorithm => throw new NotImplementedException();

        public string Name { get; }

        public IReadOnlyCollection<byte> PublicKey => throw new NotImplementedException();

        public Version Version => _assemblyReference.Version;

        public IReadOnlyDictionary<string, IAssemblyMetadata> RequiredAssemblies => throw new NotImplementedException();

        public FileInfo File => throw new NotImplementedException();

        public IReadOnlyDictionary<string, ITypeMetadata> TypeDefinitions => throw new NotImplementedException();

        public IReadOnlyDictionary<string, ITypeMetadata> TypeReferences => throw new NotImplementedException();

        public IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes => throw new NotImplementedException();
    }
}