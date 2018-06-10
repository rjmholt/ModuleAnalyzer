using System.Reflection.Metadata;
using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata
{
    public class CustomAttributeMetadata
    {
        public class Prototype
        {
            public CustomAttributeMetadata Get()
            {

            }
        }

        public CustomAttributeMetadata(
            TypeMetadata attributeType,
            IImmutableDictionary<string, CustomAttributeNamedArgument<TypeMetadata>> namedArguments,
            IImmutableList<CustomAttributeTypedArgument<TypeMetadata>> positionalArguments
        )
        {
            AttributeType = attributeType;
            NamedArguments = namedArguments;
            PositionalArguments = positionalArguments;
        }

        public TypeMetadata AttributeType { get; }

        public IImmutableDictionary<string, CustomAttributeNamedArgument<TypeMetadata>> NamedArguments { get; }

        public IImmutableList<CustomAttributeTypedArgument<TypeMetadata>> PositionalArguments { get; }
    }
}