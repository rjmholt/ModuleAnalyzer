using System;
using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Interface;

namespace MetadataAnalysis.Metadata.ILParse
{
    public abstract class ILTypeMetadata : ITypeMetadata
    {
        protected ILTypeMetadata(
            string name,
            string @namespace,
            TypeKind typeKind,
            ProtectionLevel protectionLevel,
            ITypeMetadata baseType,
            ITypeMetadata declaringType,
            IImmutableList<IConstructorMetadata> constructors,
            IImmutableDictionary<string, IFieldMetadata> fields,
            IImmutableDictionary<string, IPropertyMetadata> properties,
            IImmutableDictionary<string, IImmutableList<IMethodMetadata>> methods
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

        public ITypeMetadata BaseType { get; }

        public ITypeMetadata DeclaringType { get; }

        public IImmutableList<IConstructorMetadata> Constructors { get; }

        public IImmutableDictionary<string, IFieldMetadata> Fields { get; }

        public IImmutableDictionary<string, IPropertyMetadata> Properties { get; }

        public IImmutableDictionary<string, IImmutableList<IMethodMetadata>> Methods { get; }

        public IImmutableDictionary<string, ITypeMetadata> NestedTypes { get; internal set; }
    }
}