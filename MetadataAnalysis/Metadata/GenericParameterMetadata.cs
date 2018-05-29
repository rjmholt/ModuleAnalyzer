using System.Reflection;

namespace MetadataAnalysis.Metadata
{
    public class GenericParameterMetadata
    {
        public GenericParameterMetadata(string name, GenericParameterAttributes attributes)
        {
            Name = name;
            Attributes = attributes;
        }

        public string Name { get; }

        public GenericParameterAttributes Attributes { get; }
    }
}