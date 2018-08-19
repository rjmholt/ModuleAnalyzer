using System.IO;
using MetadataHydrator.Lazy.Metadata;

namespace MetadataHydrator.Lazy
{
    public class LazyMetadataHydrator : IMetadataHydrator
    {
        private readonly AssemblyCache _assemblyCache;

        private readonly TypeResolver _typeResolver;

        public LazyMetadataHydrator()
        {
            _assemblyCache = new AssemblyCache();
            _typeResolver = new TypeResolver();
        }

        public IAssemblyMetadata ReadAssembly(string assemblyPath)
        {
            LazyAssemblyHydrator assemblyHydrator = LazyAssemblyHydrator.Create(
                assemblyPath,
                _typeResolver,
                _assemblyCache);

            return assemblyHydrator.ReadAssembly();
        }
    }
}