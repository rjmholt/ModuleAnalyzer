using System.Collections.Generic;
using System.IO;
using MetadataHydrator.Lazy.LoadedTypes;

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

        public AssemblyDirectoryResolver GetResolverForPath(
            string assemblyPath,
            out LazyAssemblyDefinitionMetadata assembly)
        {
            var assemblyFile = new FileInfo(assemblyPath);
            if (_assemblyDirectories.TryGetValue(assemblyFile.DirectoryName, out AssemblyDirectoryResolver asmDirResolver))
            {
                assembly = asmDirResolver.GetAssemblyByFilename(assemblyFile.Name);
                return asmDirResolver;
            }

            asmDirResolver = AssemblyDirectoryResolver.Create(
                this,
                _loadedTypeResolver,
                assemblyFile,
                out assembly);
            
            _assemblyDirectories.Add(assemblyFile.DirectoryName, asmDirResolver);

            return asmDirResolver;
        }
    }
}