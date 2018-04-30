using System.Reflection;

namespace MetadataAnalysis.Metadata.Interface
{
    /// <summary>
    /// Represents the metadata of a field on a type.
    /// </summary>
    public interface IFieldMetadata : IMemberMetadata
    {
        /// <summary>
        /// The type of the field.
        /// </summary>
        ITypeMetadata Type { get; }
    }
}