using System.Reflection.Metadata;

namespace MetadataHydrator.Lazy.Metadata
{
    internal class LazyGenericParameterMetadata : IGenericParameterMetadata
    {
        private readonly GenericParameter _genericParameter;

        private readonly LazyAssemblyHydrator _assemblyHydrator;

        public LazyGenericParameterMetadata(GenericParameter genericParameter, LazyAssemblyHydrator assemblyHydrator)
        {
            _genericParameter = genericParameter;
            _assemblyHydrator = assemblyHydrator;
        }
    }
}