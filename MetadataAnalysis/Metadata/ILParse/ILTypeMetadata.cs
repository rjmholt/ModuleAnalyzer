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
            IImmutableList<IConstructorMetadata> constructors,
            IImmutableDictionary<string, IFieldMetadata> fields,
            IImmutableDictionary<string, IPropertyMetadata> properties,
            IImmutableDictionary<string, IImmutableList<IMethodMetadata>> methods,
            IImmutableDictionary<string, ITypeMetadata> nestedTypes
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
            NestedTypes = nestedTypes;
        }

        public string Name { get; }

        public string Namespace { get; }

        public TypeKind TypeKind { get; }

        public ProtectionLevel ProtectionLevel { get; }

        public ITypeMetadata BaseType { get; }

        public IImmutableList<IConstructorMetadata> Constructors { get; }

        public IImmutableDictionary<string, IFieldMetadata> Fields { get; }

        public IImmutableDictionary<string, IPropertyMetadata> Properties { get; }

        public IImmutableDictionary<string, IImmutableList<IMethodMetadata>> Methods { get; }

        public IImmutableDictionary<string, ITypeMetadata> NestedTypes { get; }
    }
}