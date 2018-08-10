using System.Collections.Generic;

namespace MetadataAnalysis.Interface
{
    public interface IMethodMetadata : IMemberMetadata
    {
        ITypeMetadata ReturnType { get; }

        IReadOnlyList<ITypeMetadata> ParameterTypes { get; }
    }
}