using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace MetadataAnalysis.Interface
{
    public interface ICustomAttributeMetadata
    {
        ITypeMetadata AttributeType { get; }

        IImmutableList<CustomAttributeTypedArgument<ITypeMetadata>> PositionalArguments { get; }

        IImmutableList<CustomAttributeNamedArgument<ITypeMetadata>> NamedArguments { get; }
    }
}