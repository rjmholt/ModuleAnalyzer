using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;

namespace MetadataAnalysis.Metadata.Generic
{
    public abstract class GenericParameterMetadata
    {
        protected GenericParameterMetadata(TypeMetadata type, string name, GenericParameterAttributes attributes)
        {
            Name = name;
            Attributes = attributes;
            Type = type;
        }

        public string Name { get; }

        public GenericParameterAttributes Attributes { get; }

        public TypeMetadata Type;
    }
    
    public class UninstantiatedGenericParameterMetadata : GenericParameterMetadata
    {
        public UninstantiatedGenericParameterMetadata(string name, GenericParameterAttributes attributes)
            : base(new UninstantiateGenericTypeMetadata(name), name, attributes)
        {
        }
    }

    public class ConcreteGenericParameterMetadata : GenericParameterMetadata
    {
        public ConcreteGenericParameterMetadata(TypeMetadata type, GenericParameterAttributes attributes)
            : base(type, type.Name, attributes)
        {
        }
    }

    public class UninstantiateGenericTypeMetadata : TypeMetadata
    {
        public UninstantiateGenericTypeMetadata(string name)
                : base(name, String.Empty, name, TypeKind.Dummy, ProtectionLevel.Public)
        {
        }

        internal override TypeMetadata InstantiateGenerics(IImmutableList<TypeMetadata> genericArguments)
        {
            throw new NotImplementedException();
        }
    }
}