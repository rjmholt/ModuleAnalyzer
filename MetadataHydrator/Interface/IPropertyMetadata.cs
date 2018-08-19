namespace MetadataHydrator
{
    public interface IPropertyMetadata : IMemberMetadata
    {
        ITypeMetadata Type { get; }
    }
}