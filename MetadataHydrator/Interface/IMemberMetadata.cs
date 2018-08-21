using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator
{
    public interface IMemberMetadata
    {
        string Name { get; }

        IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes { get; }
    }
}