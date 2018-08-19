using System.Collections.Generic;
using System.Reflection.Metadata;

namespace MetadataHydrator.Lazy.Metadata
{
    internal class LazyCustomAttributeMetadata : ICustomAttributeMetadata
    {
        private readonly CustomAttribute _customAttribute;

        private readonly LazyAssemblyHydrator _assemblyHydrator;

        public LazyCustomAttributeMetadata(CustomAttribute customAttribute, LazyAssemblyHydrator assemblyHydrator)
        {
            _customAttribute = customAttribute;
            _assemblyHydrator = assemblyHydrator;
        }

        public ITypeMetadata AttributeType => throw new System.NotImplementedException();

        public IReadOnlyCollection<CustomAttributeTypedArgument<ITypeMetadata>> FixedArguments => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, CustomAttributeNamedArgument<ITypeMetadata>> NamedArguments => throw new System.NotImplementedException();
    }
}