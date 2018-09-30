using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace MetadataHydrator.Lazy.LoadedTypes
{
    public class LoadedCustomAttributeMetadata : ICustomAttributeMetadata
    {
        private readonly CustomAttributeData _customAttribute;

        public LoadedCustomAttributeMetadata(CustomAttributeData customAttribute)
        {
            _customAttribute = customAttribute;
        }

        public ITypeDefinitionMetadata AttributeType => throw new System.NotImplementedException();

        public IReadOnlyCollection<CustomAttributeTypedArgument<ITypeDefinitionMetadata>> FixedArguments => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, CustomAttributeNamedArgument<ITypeDefinitionMetadata>> NamedArguments => throw new System.NotImplementedException();
    }
}