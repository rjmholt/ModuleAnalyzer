using System.Collections.Immutable;

namespace MetadataAnalysis.Interface
{
    public interface IMemberMetadata
    {
        string Name { get; }

        ProtectionLevel ProtectionLevel { get; }

        IImmutableList<ICustomAttributeMetadata> CustomAttributes { get; }

        IImmutableList<IGenericParameterMetadata> GenericParameters { get; }
    }
}