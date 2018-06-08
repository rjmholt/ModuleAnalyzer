using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public abstract class TypeMetadata
    {
        protected TypeMetadata(
            string name,
            string @namespace,
            TypeKind typeKind,
            ProtectionLevel protectionLevel,
            TypeMetadata baseType,
            IImmutableList<ConstructorMetadata> constructors,
            IImmutableDictionary<string, FieldMetadata> fields,
            IImmutableDictionary<string, PropertyMetadata> properties,
            IImmutableDictionary<string, IImmutableList<MethodMetadata>> methods,
            IImmutableList<GenericParameterMetadata> genericParameters = null,
            IImmutableList<CustomAttributeMetadata> customAttributes = null
        )
        {
            Name = name;
            Namespace = @namespace;
            TypeKind = TypeKind;
            ProtectionLevel = protectionLevel;
            BaseType = baseType;
            Fields = fields;
            Properties = properties;
            Methods = methods;
            GenericParameters = genericParameters ?? ImmutableArray<GenericParameterMetadata>.Empty;
            CustomAttributes = customAttributes ?? ImmutableArray<CustomAttributeMetadata>.Empty;
        }

        public string Name { get; }

        public string Namespace { get; }

        public virtual string FullName
        {
            get
            {
                if (String.IsNullOrEmpty(Namespace))
                {
                    return Name;
                }

                return Namespace + "." + Name;
            }
        }

        public TypeKind TypeKind { get; }

        public ProtectionLevel ProtectionLevel { get; }

        public TypeMetadata BaseType { get; }

        public IImmutableList<ConstructorMetadata> Constructors { get; }

        public IImmutableDictionary<string, FieldMetadata> Fields { get; }

        public IImmutableDictionary<string, PropertyMetadata> Properties { get; }

        public IImmutableDictionary<string, IImmutableList<MethodMetadata>> Methods { get; }

        public IImmutableList<GenericParameterMetadata> GenericParameters { get; }

        public IImmutableList<CustomAttributeMetadata> CustomAttributes { get; }

        public bool IsGeneric { get => GenericParameters.Any(); }

        public bool IsFullyInstantiated()
        {
            foreach (GenericParameterMetadata genericParameter in GenericParameters)
            {
                if (genericParameter is UninstantiatedGenericParameterMetadata)
                {
                    return false;
                }
            }

            return true;
        }

        protected IImmutableList<GenericParameterMetadata> InstantiateGenericListAtIndex(
            NameableTypeMetadata parameterType,
            int index)
        {
            if (GenericParameters[index] is ConcreteGenericParameterMetadata)
            {
                throw new ArgumentException("Cannot instantiate concrete generic parameter");
            }

            List<GenericParameterMetadata> genericParameters = GenericParameters.ToList();
            genericParameters[index] = new ConcreteGenericParameterMetadata(parameterType, GenericParameters[index].Attributes);
            return genericParameters.ToImmutableArray();
        }
    }
}