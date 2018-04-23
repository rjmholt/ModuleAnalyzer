using System.Reflection;

namespace MetadataAnalysis.Metadata.Interface
{
    public interface IFieldMetadata : IMemberMetadata
    {
         ITypeMetadata Type { get; }
    }
}