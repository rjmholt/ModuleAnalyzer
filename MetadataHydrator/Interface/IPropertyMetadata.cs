namespace MetadataHydrator
{
    public interface IPropertyMetadata : IMemberMetadata
    {
        ITypeReferenceMetadata Type { get; }
    }
}