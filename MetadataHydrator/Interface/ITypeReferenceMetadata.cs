namespace MetadataHydrator
{
    public interface ITypeReferenceMetadata
    {
        string Name { get; }

        string Namespace { get; }

        string FullName { get; }
    }
}