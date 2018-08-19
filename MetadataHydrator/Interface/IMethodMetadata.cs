using System.Collections.Generic;

namespace MetadataHydrator
{
    public interface IMethodMetadata : IMemberMetadata
    {
        ITypeMetadata ReturnType { get; }

        IReadOnlyCollection<ITypeMetadata> ParameterTypes { get; }
    }
}