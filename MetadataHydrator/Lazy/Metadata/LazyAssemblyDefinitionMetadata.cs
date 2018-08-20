using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;

namespace MetadataHydrator.Lazy.Metadata
{
    internal class LazyAssemblyDefinitionMetadata : IAssemblyMetadata
    {
        private readonly LazyAssemblyHydrator _assemblyHydrator;

        private readonly AssemblyDefinition _assemblyDefinition;

        private string _culture;

        private string _name;

        private IReadOnlyDictionary<string, ITypeMetadata> _definedTypes;

        private IReadOnlyCollection<byte> _publicKey;

        private IReadOnlyDictionary<string, IAssemblyMetadata> _requiredAssemblies;

        private IReadOnlyCollection<LazyCustomAttributeMetadata> _customAttributes;

        public LazyAssemblyDefinitionMetadata(
            AssemblyDefinition assemblyDefinition,
            LazyAssemblyHydrator assemblyHydrator)
        {
            _assemblyHydrator = assemblyHydrator;
            _assemblyDefinition = assemblyDefinition;
        }

        public string Culture
        {
            get
            {
                if (_culture == null)
                {
                    _culture = _assemblyHydrator.ReadAssemblyCulture(_assemblyDefinition);
                }
                return _culture;
            }
        }

        public AssemblyFlags Flags => _assemblyDefinition.Flags;

        public AssemblyHashAlgorithm HashAlgorithm => _assemblyDefinition.HashAlgorithm;

        public string Name
        {
            get
            {
                if (_name == null)
                {
                    _name = _assemblyHydrator.ReadAssemblyName(_assemblyDefinition);
                }
                return _name;
            }
        }

        public IReadOnlyCollection<byte> PublicKey
        {
            get
            {
                if (_publicKey == null)
                {
                    _publicKey = _assemblyHydrator.ReadAssemblyPublicKey(_assemblyDefinition);
                }
                return _publicKey;
            }
        }

        public Version Version => _assemblyDefinition.Version;

        public IReadOnlyDictionary<string, IAssemblyMetadata> RequiredAssemblies
        {
            get
            {
                if (_requiredAssemblies == null)
                {
                    _requiredAssemblies = _assemblyHydrator.ReadRequiredAssemblies();
                }
                return _requiredAssemblies;
            }
        }

        public FileInfo File
        {
            get
            {
                return _assemblyHydrator.AssemblyFile;
            }
        }

        public IReadOnlyDictionary<string, ITypeMetadata> TypeDefinitions
        {
            get
            {
                if (_definedTypes == null)
                {
                    _definedTypes = _assemblyHydrator.ReadDefinedTypes();
                }
                return _definedTypes;
            }
        }

        public IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes
        {
            get
            {
                if (_customAttributes == null)
                {
                    _customAttributes = _assemblyHydrator.ReadCustomAttributes(_assemblyDefinition.GetCustomAttributes());
                }
                return _customAttributes;
            }
        }

        public IReadOnlyDictionary<string, ITypeMetadata> TypeReferences => throw new NotImplementedException();
    }
}