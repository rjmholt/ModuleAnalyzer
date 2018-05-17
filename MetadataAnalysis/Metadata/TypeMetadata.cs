using System;
using System.Collections.Immutable;

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
            TypeMetadata declaringType,
            IImmutableList<ConstructorMetadata> constructors,
            IImmutableDictionary<string, FieldMetadata> fields,
            IImmutableDictionary<string, PropertyMetadata> properties,
            IImmutableDictionary<string, IImmutableList<MethodMetadata>> methods
        )
        {
            Name = name;
            Namespace = @namespace;
            TypeKind = TypeKind;
            ProtectionLevel = protectionLevel;
            BaseType = baseType;
            DeclaringType = declaringType;
            Fields = fields;
            Properties = properties;
            Methods = methods;
        }

        public string Name { get; }

        public string Namespace { get; }

        public string FullName
        {
            get
            {
                if (DeclaringType != null)
                {
                    return DeclaringType.FullName + "." + Name;
                }

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

        public TypeMetadata DeclaringType { get; }

        public IImmutableList<ConstructorMetadata> Constructors { get; }

        public IImmutableDictionary<string, FieldMetadata> Fields { get; }

        public IImmutableDictionary<string, PropertyMetadata> Properties { get; }

        public IImmutableDictionary<string, IImmutableList<MethodMetadata>> Methods { get; }

        public IImmutableDictionary<string, TypeMetadata> NestedTypes { get; internal set; }
    }
}