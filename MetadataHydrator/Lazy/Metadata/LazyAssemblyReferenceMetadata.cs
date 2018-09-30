using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;

namespace MetadataHydrator.Lazy.Metadata
{
    internal class LazyAssemblyReferenceMetadata : IAssemblyReferenceMetadata, IAssemblyDefinitionMetadata
    {
        private readonly AssemblyReference _assemblyReference;

        private readonly LazyAssemblyHydrator _assemblyHydrator;

        private readonly Lazy<string> _culture;

        private readonly Lazy<IReadOnlyCollection<byte>> _hashValue;

        private readonly Lazy<IReadOnlyCollection<byte>> _publicToken;

        public LazyAssemblyReferenceMetadata(
            string assemblyName,
            AssemblyReference assemblyReference,
            LazyAssemblyHydrator assemblyHydrator)
        {
            _assemblyReference = assemblyReference;
            _assemblyHydrator = assemblyHydrator;
            Name = assemblyName;
            _culture = new Lazy<string>(() => _assemblyHydrator.ReadString(_assemblyReference.Culture));
            _hashValue = new Lazy<IReadOnlyCollection<byte>>(() => _assemblyHydrator.ReadBlob(_assemblyReference.HashValue));
            _publicToken = new Lazy<IReadOnlyCollection<byte>>(() => _assemblyHydrator.ReadBlob(_assemblyReference.PublicKeyOrToken));
        }

        public string Culture
        {
            get => _culture.Value;
        }

        public AssemblyFlags Flags => _assemblyReference.Flags;

        public string Name { get; }

        public Version Version => _assemblyReference.Version;

        public IReadOnlyCollection<byte> HashValue => _hashValue.Value;

        public IReadOnlyCollection<byte> PublicKeyOrToken => _publicToken.Value;

        AssemblyHashAlgorithm IAssemblyDefinitionMetadata.HashAlgorithm => throw new NotImplementedException();

        IReadOnlyCollection<byte> IAssemblyDefinitionMetadata.PublicKey => throw new NotImplementedException();

        IReadOnlyDictionary<string, IAssemblyDefinitionMetadata> IAssemblyDefinitionMetadata.RequiredAssemblies => throw new NotImplementedException();

        FileInfo IAssemblyDefinitionMetadata.File => throw new NotImplementedException();

        IReadOnlyDictionary<string, ITypeDefinitionMetadata> IAssemblyDefinitionMetadata.TypeDefinitions => throw new NotImplementedException();

        IReadOnlyDictionary<string, ITypeReferenceMetadata> IAssemblyDefinitionMetadata.TypeReferences => throw new NotImplementedException();

        IReadOnlyCollection<ICustomAttributeMetadata> IAssemblyDefinitionMetadata.CustomAttributes => throw new NotImplementedException();
    }
}