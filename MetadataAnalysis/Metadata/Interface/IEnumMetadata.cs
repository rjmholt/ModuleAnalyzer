using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata.Interface
{
    public interface IEnumMetadata : ITypeMetadata
    {
        IImmutableList<string> Members { get; }
    }
}