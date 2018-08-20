using System.IO;
using MetadataHydrator.Lazy.Metadata;

namespace MetadataHydrator.Lazy
{
    public class LazyMetadataHydrator : IMetadataHydrator
    {
        private readonly AssemblyResolver _assemblyResolver;

        public LazyMetadataHydrator()
        {
            _assemblyResolver = new AssemblyResolver();
        }

        public IAssemblyMetadata ReadAssembly(string assemblyPath)
        {
            AssemblyDirectoryResolver asmDirResolver = _assemblyResolver.GetResolverForPath(assemblyPath);
            string assemblyFileName = Path.GetFileName(assemblyPath);
            return asmDirResolver.GetAssemblyByFileName(assemblyFileName);
        }
    }
}