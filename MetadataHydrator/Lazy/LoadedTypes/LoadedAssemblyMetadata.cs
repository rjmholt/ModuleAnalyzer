using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MetadataHydrator.Lazy.LoadedTypes
{
    public class LoadedAssemblyMetadata : IAssemblyDefinitionMetadata
    {
        private readonly Assembly _assembly;

        private readonly AssemblyName _assemblyName;

        private readonly Lazy<FileInfo> _asmFile;

        public LoadedAssemblyMetadata(Assembly assembly)
        {
            _assembly = assembly;
            _assemblyName = assembly.GetName();
            _asmFile = new Lazy<FileInfo>(() => new FileInfo(new Uri(_assembly.CodeBase).LocalPath));
        }

        public string Culture => _assemblyName.CultureName;

        public AssemblyFlags Flags
        {
            get
            {
                AssemblyFlags flags = (AssemblyFlags)_assemblyName.Flags;
                if (_assemblyName.ContentType == AssemblyContentType.WindowsRuntime)
                {
                    flags |= AssemblyFlags.WindowsRuntime;
                }
                return flags;
            }
        }

        public AssemblyHashAlgorithm HashAlgorithm => (AssemblyHashAlgorithm)_assemblyName.HashAlgorithm;

        public string Name => _assemblyName.Name;

        public IReadOnlyCollection<byte> PublicKey => _assemblyName.GetPublicKey();

        public Version Version => _assemblyName.Version;

        public IReadOnlyDictionary<string, IAssemblyDefinitionMetadata> RequiredAssemblies => throw new NotImplementedException();

        public FileInfo File => _asmFile.Value;

        public IReadOnlyDictionary<string, ITypeDefinitionMetadata> TypeDefinitions => throw new NotImplementedException();

        public IReadOnlyDictionary<string, ITypeReferenceMetadata> TypeReferences => throw new NotImplementedException();

        public IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes => throw new NotImplementedException();
    }
}