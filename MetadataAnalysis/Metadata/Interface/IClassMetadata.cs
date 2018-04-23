namespace MetadataAnalysis.Metadata.Interface
{
    public interface IClassMetadata : ITypeMetadata
    {
         bool IsStatic { get; }

         bool IsAbstract { get; }

         bool IsSealed { get; }
    }
}