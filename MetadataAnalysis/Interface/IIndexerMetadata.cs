namespace MetadataAnalysis.Interface
{
    public interface IIndexerMetadata : IMemberMetadata
    {
        ITypeMetadata Type { get; }

        ITypeMetadata IndexType { get; }

        IIndexerGetterMetadata Getter { get; }

        IIndexerSetterMetadata Setter { get; }
    }
}