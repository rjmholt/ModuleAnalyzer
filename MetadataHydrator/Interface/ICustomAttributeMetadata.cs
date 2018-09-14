using System.Collections.Generic;
using System.Reflection.Metadata;

namespace MetadataHydrator
{
    public interface ICustomAttributeMetadata
    {
        ITypeDefinitionMetadata AttributeType { get; }

        IReadOnlyCollection<CustomAttributeTypedArgument<ITypeDefinitionMetadata>> FixedArguments { get; }

        IReadOnlyDictionary<string, CustomAttributeNamedArgument<ITypeDefinitionMetadata>> NamedArguments { get; }
    }
}