using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata.Interface
{
    /// <summary>
    /// Represents the metadata of an enumeration type.
    /// </summary>
    public interface IEnumMetadata : ITypeMetadata
    {
        /// <summary>
        /// The possible values of the enumeration.
        /// </summary>
        IImmutableList<string> Members { get; } // TODO: Make these into their own object to capture custom attributes
    }
}