using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MetadataAnalysis.Metadata.Generic;
using MetadataAnalysis.Metadata.Prototypes;

namespace MetadataAnalysis.Metadata
{
    public abstract class TypeMetadata
    {
        protected TypeMetadata(
            string name,
            string @namespace,
            string fullName,
            TypeKind typeKind,
            ProtectionLevel protectionLevel)
        {
            Name = name;
            Namespace = @namespace;
            FullName = fullName;
            TypeKind = typeKind;
            ProtectionLevel = protectionLevel;
        }

        public string Name { get; }

        public string Namespace { get; }

        public string FullName { get; }

        public TypeKind TypeKind { get; }

        public ProtectionLevel ProtectionLevel { get; }

        public TypeMetadata BaseType { get; internal set; }

        public IImmutableList<ConstructorMetadata> Constructors { get; internal set;}

        public IImmutableDictionary<string, FieldMetadata> Fields { get; internal set; }

        public IImmutableDictionary<string, PropertyMetadata> Properties { get; internal set; }

        public IImmutableList<IndexerMetadata> Indexers { get; internal set; }

        public IImmutableDictionary<string, IImmutableList<MethodMetadata>> Methods { get; internal set; }

        public IImmutableList<GenericParameterMetadata> GenericParameters { get; internal set; }

        public IImmutableList<CustomAttributeMetadata> CustomAttributes { get; internal set; }

        public bool IsGeneric { get => GenericParameters.Any(); }

        public bool IsInstantiated()
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

        internal abstract TypeMetadata InstantiateGenerics(IImmutableList<TypeMetadata> genericArguments);

        internal IImmutableList<GenericParameterMetadata> InstantiateGenericList(
            IImmutableList<TypeMetadata> genericArguments)
        {
            var genericParameters = new List<GenericParameterMetadata>(GenericParameters);

            for (int i = 0; i < genericArguments.Count; i++)
            {
                genericParameters[i] = new ConcreteGenericParameterMetadata(
                    genericArguments[i],
                    GenericParameters[i].Attributes);
            }

            return genericParameters.ToImmutableArray();
        }
    }
}
