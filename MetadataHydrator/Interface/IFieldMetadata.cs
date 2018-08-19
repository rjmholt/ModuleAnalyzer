namespace MetadataHydrator
{
    public interface IFieldMetadata : IMemberMetadata
    {
        ITypeMetadata Type { get; }
    }
}