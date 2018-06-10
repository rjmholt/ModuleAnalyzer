using System.Collections.Generic;

namespace MetadataAnalysis.Metadata.Prototypes
{
    public abstract class TypeMetadataPrototype<TType> where TType : TypeMetadata
    {
        protected TypeMetadataPrototype(string name, string @namespace, TypeKind typeKind, ProtectionLevel protectionLevel)
        {
            Name = name;
            Namespace = @namespace;
            TypeKind = typeKind;
            ProtectionLevel = protectionLevel;
            NestedTypes = new List<ITypeMetadataPrototype<TypeMetadata>>();
        }

        public string Name { get; }

        public string Namespace { get; }

        public TypeKind TypeKind { get; }

        public ProtectionLevel ProtectionLevel { get; }

        public ITypeMetadataPrototype<TypeMetadata> BaseType { get; set; }

        public IList<ITypeMetadataPrototype<TypeMetadata>> NestedTypes { get; }
    }
}