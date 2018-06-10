using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace MetadataAnalysis.Metadata.Generic
{
    public abstract class GenericParameterMetadata
    {
        public abstract class Prototype
        {
            public abstract GenericParameterMetadata Get();
        }

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
        public new class Prototype : GenericParameterMetadata.Prototype
        {
            public override GenericParameterMetadata Get()
            {

            }
        }

        public UninstantiatedGenericParameterMetadata(string name, GenericParameterAttributes attributes)
            : base(name, attributes)
        {
        }
    }

    public class ConcreteGenericParameterMetadata : GenericParameterMetadata
    {
        public new class Prototype : GenericParameterMetadata.Prototype
        {
            public override GenericParameterMetadata Get()
            {

            }
        }

        public ConcreteGenericParameterMetadata(TypeMetadata type, GenericParameterAttributes attributes)
            : base(type.Name, attributes)
        {
            Type = type;
        }

        TypeMetadata Type { get; }
    }
}