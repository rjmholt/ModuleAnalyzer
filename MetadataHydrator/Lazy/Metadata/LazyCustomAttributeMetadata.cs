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

        public ITypeDefinitionMetadata AttributeType => throw new System.NotImplementedException();

        public IReadOnlyCollection<CustomAttributeTypedArgument<ITypeDefinitionMetadata>> FixedArguments => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, CustomAttributeNamedArgument<ITypeDefinitionMetadata>> NamedArguments => throw new System.NotImplementedException();
    }
}