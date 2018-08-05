using System.Reflection;

namespace MetadataAnalysis.Interface
{
    public interface IGenericParameterMetadata
    {
        string Name { get; }

        ITypeMetadata Type { get; }

        GenericParameterAttributes Attributes { get; }
    }
}