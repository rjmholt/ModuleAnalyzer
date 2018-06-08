using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace MetadataAnalysis.Metadata.Generic
{
    public abstract class GenericParameterMetadata
    {
        protected GenericParameterMetadata(string name, GenericParameterAttributes attributes)
        {
            Name = name;
            Attributes = attributes;
        }

        public string Name { get; }

        public GenericParameterAttributes Attributes { get; }
    }
    
    public class UninstantiatedGenericParameterMetadata : GenericParameterMetadata
    {
        public UninstantiatedGenericParameterMetadata(string name, GenericParameterAttributes attributes)
            : base(name, attributes)
        {
        }
    }

    public class ConcreteGenericParameterMetadata : GenericParameterMetadata
    {
        public ConcreteGenericParameterMetadata(NameableTypeMetadata type, GenericParameterAttributes attributes)
            : base(type.Name, attributes)
        {
            Type = type;
        }

        NameableTypeMetadata Type { get; }
    }
}