using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator.Lazy.Metadata
{
    internal abstract class LazyMemberMetadata : IMemberMetadata
    {
        protected readonly LazyAssemblyHydrator _assemblyHydrator;

        protected LazyMemberMetadata(
            string name,
            LazyAssemblyHydrator assemblyHydrator)
        {
            _assemblyHydrator = assemblyHydrator;
            Name = name;
        }

        public string Name { get; }

        public abstract IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes { get; }

        protected abstract void ResolveSignature();
    }
}