using System.Collections.Generic;

namespace MetadataAnalysis.Metadata.Prototypes
{
    internal abstract class DefinedTypeMetadataPrototype<TType> : TypeMetadataPrototype<TType>
        where TType : TypeMetadata
    {
        protected DefinedTypeMetadataPrototype(
            string name,
            string @namespace,
            TypeKind typeKind,
            ProtectionLevel protectionLevel)
                : base(name, @namespace, typeKind, protectionLevel)
        {
        }

        public TypeMetadataPrototype<TypeMetadata> baseType;

        public IList<TypeMetadataPrototype<TypeMetadata>> nestedTypes;
    }
}