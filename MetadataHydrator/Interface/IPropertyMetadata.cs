namespace MetadataHydrator
{
    public interface IPropertyMetadata : IMemberMetadata
    {
        ITypeDefinitionMetadata Type { get; }
    }
}