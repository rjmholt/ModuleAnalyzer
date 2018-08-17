namespace MetadataHydrator.Interface
{
    public interface ICustomAttribute
    {
        ITypeMetadata AttributeType { get; }

        IReadOnlyCollection<CustomAttributeTypedArgument<ITypeMetadata>> FixedArguments { get; }

        IReadOnlyDictionary<string, CustomAttributeNamedArgument<ITypeMetadata>> NamedArguments { get; }
    }
}