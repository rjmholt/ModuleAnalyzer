using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace MetadataAnalysis.Interface
{
    public interface ICustomAttributeMetadata
    {
        ITypeMetadata AttributeType { get; }

        IReadOnlyList<CustomAttributeTypedArgument<ITypeMetadata>> PositionalArguments { get; }

        IReadOnlyList<CustomAttributeNamedArgument<ITypeMetadata>> NamedArguments { get; }
    }
}