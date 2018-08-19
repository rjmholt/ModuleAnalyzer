using System.Collections.Generic;
using System.Reflection.Metadata;

namespace MetadataHydrator
{
    public interface ICustomAttributeMetadata
    {
        ITypeMetadata AttributeType { get; }

        IReadOnlyCollection<CustomAttributeTypedArgument<ITypeMetadata>> FixedArguments { get; }

        IReadOnlyDictionary<string, CustomAttributeNamedArgument<ITypeMetadata>> NamedArguments { get; }
    }
}