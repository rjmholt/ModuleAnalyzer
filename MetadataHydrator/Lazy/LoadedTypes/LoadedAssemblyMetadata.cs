using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace MetadataHydrator.Lazy.LoadedTypes
{
    public class LoadedAssemblyMetadata : LoadedAssemblyReference, IAssemblyDefinitionMetadata
    {
        private readonly Assembly _assembly;

        private readonly Lazy<FileInfo> _asmFile;

        private readonly Lazy<IDictionary<string, LoadedAssemblyMetadata>> _referencedAssemblies;

        private readonly Lazy<IDictionary<string, LoadedTypeMetadata>> _definedTypes;

        private readonly Lazy<IDictionary<string, LoadedTypeMetadata>> _referencedTypes;

        private readonly Lazy<IList<LoadedCustomAttributeMetadata>> _customAttributes;

        public LoadedAssemblyMetadata(Assembly assembly) : base(assembly.GetName())
        {
            _assembly = assembly;
            _asmFile = new Lazy<FileInfo>(() => new FileInfo(new Uri(_assembly.CodeBase).LocalPath));
            _referencedAssemblies = new Lazy<IDictionary<string, LoadedAssemblyReference>>(() => _assembly.GetReferencedAssemblies().ToDictionary(ra => ra.FullName, ra => new LoadedAssemblyReference(ra));
            _definedTypes = new Lazy<IDictionary<string, LoadedTypeMetadata>>(() => _assembly.DefinedTypes.ToDictionary(t => t.FullName, t => new LoadedTypeMetadata(t)));
            _customAttributes = new Lazy<LoadedCustomAttributeMetadata[]>(() => _assembly.CustomAttributes.Select(ca => new LoadedCustomAttributeMetadata(ca)).ToArray());
        }

        public AssemblyHashAlgorithm HashAlgorithm => (AssemblyHashAlgorithm)_assemblyName.HashAlgorithm;

        public IReadOnlyCollection<byte> PublicKey => _assemblyName.GetPublicKey();

        public IReadOnlyDictionary<string, IAssemblyDefinitionMetadata> RequiredAssemblies => (IReadOnlyDictionary<string, IAssemblyDefinitionMetadata>)_referencedAssemblies.Value;

        public FileInfo File => _asmFile.Value;

        public IReadOnlyDictionary<string, ITypeDefinitionMetadata> TypeDefinitions => (IReadOnlyDictionary<string, ITypeDefinitionMetadata>)_definedTypes.Value;

        public IReadOnlyDictionary<string, ITypeReferenceMetadata> TypeReferences => (IReadOnlyDictionary<string, ITypeReferenceMetadata>)_referencedTypes.Value;

        public IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes => (IReadOnlyCollection<ICustomAttributeMetadata>)_customAttributes.Value;

        public LoadedTypeMetadata LookupType(Type type)
        {
            if (_definedTypes.Value.TryGetValue(type.FullName, out LoadedTypeMetadata typeMetadata))
            {
                return typeMetadata;
            }

            throw new Exception($"Type '{type.FullName}' not found in loaded assembly '{_assembly.FullName}'");
        }
    }
}