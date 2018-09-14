using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;

namespace MetadataHydrator.Lazy.Metadata
{
    internal class LazyAssemblyDefinitionMetadata : IAssemblyDefinitionMetadata
    {
        private readonly LazyAssemblyHydrator _assemblyHydrator;

        private readonly AssemblyDefinition _assemblyDefinition;

        private readonly Lazy<string> _culture;

        private readonly Lazy<string> _name;

        private readonly Lazy<IReadOnlyDictionary<string, ITypeDefinitionMetadata>> _definedTypes;

        private readonly Lazy<IReadOnlyCollection<byte>> _publicKey;

        private readonly Lazy<IReadOnlyDictionary<string, IAssemblyDefinitionMetadata>> _requiredAssemblies;

        private readonly Lazy<IReadOnlyCollection<ICustomAttributeMetadata>> _customAttributes;

        public LazyAssemblyDefinitionMetadata(
            AssemblyDefinition assemblyDefinition,
            LazyAssemblyHydrator assemblyHydrator)
        {
            _assemblyHydrator = assemblyHydrator;
            _assemblyDefinition = assemblyDefinition;

            _culture = new Lazy<string>(() => _assemblyHydrator.ReadString(_assemblyDefinition.Culture));
            _name = new Lazy<string>(() => _assemblyHydrator.ReadString(_assemblyDefinition.Name));
            _publicKey = new Lazy<IReadOnlyCollection<byte>>(() => _assemblyHydrator.ReadBlob(_assemblyDefinition.PublicKey));
            _definedTypes = new Lazy<IReadOnlyDictionary<string, ITypeDefinitionMetadata>>(() => _assemblyHydrator.ReadDefinedTypes());
            _requiredAssemblies = new Lazy<IReadOnlyDictionary<string, IAssemblyDefinitionMetadata>>(() => _assemblyHydrator.ReadReferencedAssemblies());
            _customAttributes = new Lazy<IReadOnlyCollection<ICustomAttributeMetadata>>(() => _assemblyHydrator.ReadCustomAttributes(_assemblyDefinition.GetCustomAttributes()));
        }

        public string Culture
        {
            get => _culture.Value;
        }

        public AssemblyFlags Flags => _assemblyDefinition.Flags;

        public AssemblyHashAlgorithm HashAlgorithm => _assemblyDefinition.HashAlgorithm;

        public string Name
        {
            get => _name.Value;
        }

        public IReadOnlyCollection<byte> PublicKey
        {
            get => _publicKey.Value;
        }

        public Version Version => _assemblyDefinition.Version;

        public IReadOnlyDictionary<string, IAssemblyDefinitionMetadata> RequiredAssemblies
        {
            get => _requiredAssemblies.Value;
        }

        public FileInfo File
        {
            get => _assemblyHydrator.AssemblyFile;
        }

        public IReadOnlyDictionary<string, ITypeDefinitionMetadata> TypeDefinitions
        {
            get => _definedTypes.Value;
        }

        public IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes
        {
            get => _customAttributes.Value;
        }

        public IReadOnlyDictionary<string, ITypeDefinitionMetadata> TypeReferences => throw new NotImplementedException();
    }
}