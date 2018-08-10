using System.Collections.Generic;

namespace MetadataAnalysis.Interface
{
    public interface IMemberMetadata
    {
        string Name { get; }

        ProtectionLevel ProtectionLevel { get; }

        IReadOnlyList<ICustomAttributeMetadata> CustomAttributes { get; }

        IReadOnlyList<IGenericParameterMetadata> GenericParameters { get; }
    }
}