namespace MetadataAnalysis.Interface
{
    public interface IPropertyMetadata : IMemberMetadata
    {
        ITypeMetadata Type { get; }

        IGetterMetadata Getter { get; }

        ISetterMetadata Setter { get; }
    }
}