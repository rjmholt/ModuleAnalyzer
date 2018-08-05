using System.Collections.Immutable;

namespace MetadataAnalysis.Interface
{
    public interface IMethodMetadata : IMemberMetadata
    {
        ITypeMetadata ReturnType { get; }

        IImmutableList<ITypeMetadata> ParameterTypes { get; }
    }
}