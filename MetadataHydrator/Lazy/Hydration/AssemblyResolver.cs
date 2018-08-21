using System.Collections.Generic;
using System.IO;
using MetadataHydrator.Lazy.LoadedTypes;
using MetadataHydrator.Lazy.Metadata;

namespace MetadataHydrator.Lazy
{
    internal class AssemblyResolver
    {
        private IDictionary<string, AssemblyDirectoryResolver> _assemblyDirectories;

        private LoadedTypeResolver _loadedTypeResolver;

        public AssemblyResolver()
        {
            _assemblyDirectories = new Dictionary<string, AssemblyDirectoryResolver>();
            _loadedTypeResolver = new LoadedTypeResolver();
        }

        public AssemblyDirectoryResolver GetResolverForPath(string assemblyPath)
        {
            var assemblyFile = new FileInfo(assemblyPath);
            if (_assemblyDirectories.TryGetValue(assemblyFile.DirectoryName, out AssemblyDirectoryResolver asmDirResolver))
            {
                return asmDirResolver;
            }

            asmDirResolver = AssemblyDirectoryResolver.Create(
                this,
                _loadedTypeResolver,
                assemblyFile);
            
            _assemblyDirectories.Add(assemblyFile.DirectoryName, asmDirResolver);

            return asmDirResolver;
        }
    }
}