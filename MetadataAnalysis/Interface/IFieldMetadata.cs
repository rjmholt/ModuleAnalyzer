namespace MetadataAnalysis.Interface
{
    public interface IFieldMetadata : IMemberMetadata
    {
        ITypeMetadata Type { get; }
    }
}