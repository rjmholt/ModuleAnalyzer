using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public abstract class TypeMetadata
    {
        public abstract class Prototype<TType, TPrototype> : IPrototype<TType>
            where TType : TypeMetadata
            where TPrototype : Prototype<TType, TPrototype>
        {
            protected Prototype(string name, string @namespace, TypeKind typeKind, ProtectionLevel protectionLevel)
            {
                this.name = name;
                this.@namespace = @namespace;
                this.typeKind = typeKind;
                this.protectionLevel = protectionLevel;

                this.constructors = new List<ConstructorMetadata.Prototype>();
                this.fields = new Dictionary<string, FieldMetadata.Prototype>();
                this.properties = new Dictionary<string, PropertyMetadata.Prototype>();
                this.genericParameters = new List<GenericParameterMetadata.Prototype>();
                this.customAttributes = new List<CustomAttributeMetadata.Prototype>();
            }

            public readonly string name;
            public readonly string @namespace;
            public readonly TypeKind typeKind;
            public readonly ProtectionLevel protectionLevel;

            public TypeMetadata.Prototype<TypeMetadata>  baseType;
            public IList<ConstructorMetadata.Prototype> constructors;
            public IDictionary<string, FieldMetadata.Prototype> fields;
            public IDictionary<string, PropertyMetadata.Prototype> properties;
            public IDictionary<string, IList<MethodMetadata.Prototype>> methods;
            public IList<GenericParameterMetadata.Prototype> genericParameters;
            public IList<CustomAttributeMetadata.Prototype> customAttributes;

            public abstract TType Get();
        }

        private readonly TypeMetadata.Prototype _prototype;

        protected TypeMetadata(TypeMetadata.Prototype prototype)
        {
            Name = prototype.name;
            Namespace = prototype.@namespace;
            TypeKind = prototype.typeKind;
            ProtectionLevel = prototype.protectionLevel;
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

        public TypeMetadata BaseType { get => _baseType ?? (_baseType = _prototype.baseType.Get()); }
        protected TypeMetadata _baseType;

        public IImmutableList<ConstructorMetadata> Constructors
        { 
            get
            {
                if (_constructors == null)
                {
                    var ctors = new List<ConstructorMetadata>();
                    foreach (ConstructorMetadata.Prototype ctor in _prototype.constructors)
                    {
                        ctors.Add((ConstructorMetadata)ctor.Get());
                    }
                    _constructors = ctors.ToImmutableArray();
                }

                return _constructors;
            }
        }
        private IImmutableList<ConstructorMetadata> _constructors;

        public IImmutableDictionary<string, FieldMetadata> Fields
        {
            get
            {
                if (_fields == null)
                {
                    _fields = ConvertPrototypeDict<string, FieldMetadata>(_prototype.fields);
                }
                return _fields;
            }
        }
        private IImmutableDictionary<string, FieldMetadata> _fields;

        public IImmutableDictionary<string, PropertyMetadata> Properties
        {
            get
            {
                if (_properties == null)
                {
                }
            }
        }
        private IImmutableDictionary<string, PropertyMetadata> _properties;

        public IImmutableDictionary<string, IImmutableList<MethodMetadata>> Methods { get; internal set; }

        public IImmutableList<GenericParameterMetadata> GenericParameters { get; internal set; }

        public IImmutableList<CustomAttributeMetadata> CustomAttributes { get; internal set; }

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

        protected static IImmutableList<T> ConvertPrototypeList<T>(IEnumerable<IPrototype<T>> prototypes)
        {
            var list = new List<T>();
            foreach (IPrototype<T> prototype in prototypes)
            {
                list.Add((T)prototype.Get());
            }
            return list.ToImmutableArray();
        }

        protected static IImmutableDictionary<TKey, T> ConvertPrototypeDict<TKey, T>(
            IDictionary<TKey, IPrototype<T>> protoDict)
        {
            var dict = new Dictionary<TKey, T>();
            foreach (KeyValuePair<TKey, IPrototype<T>> prototype in protoDict)
            {
                dict.Add(prototype.Key, prototype.Value.Get());
            }
            return dict.ToImmutableDictionary();
        }
    }
}
