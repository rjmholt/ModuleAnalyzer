using System;
using System.Collections.Generic;
using System.Reflection;

namespace MetadataHydrator.Lazy.LoadedTypes
{
    public class LoadedAssemblyReference : IAssemblyReferenceMetadata
    {
        protected readonly AssemblyName _assemblyName;

        public LoadedAssemblyReference(AssemblyName assemblyName)
        {
            _assemblyName = assemblyName;
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

        public string Name => _assemblyName.Name;

        public Version Version => _assemblyName.Version;

        public IReadOnlyCollection<byte> HashValue => throw new NotImplementedException();

        public IReadOnlyCollection<byte> PublicKeyOrToken => throw new NotImplementedException();
    }
}