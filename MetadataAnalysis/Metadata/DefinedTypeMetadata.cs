using System.Collections.Generic;
using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public abstract class DefinedTypeMetadata : TypeMetadata
    {
        public abstract new class Prototype : TypeMetadata.Prototype
        {
            protected Prototype(string name, string @namespace, TypeKind typeKind, ProtectionLevel protectionLevel)
                : base(name, @namespace, typeKind, protectionLevel)
            {
                nestedTypes = new Dictionary<string, Prototype>();
            }

            public DefinedTypeMetadata.Prototype declaringType;

            public IDictionary<string, DefinedTypeMetadata.Prototype> nestedTypes;
        }

        protected DefinedTypeMetadata(DefinedTypeMetadata.Prototype prototype)
                : base(prototype)
        {
            DeclaringType = declaringType;
        }

        public DefinedTypeMetadata DeclaringType { get; }

        public IImmutableDictionary<string, DefinedTypeMetadata> NestedTypes { get; internal set; }
    }
}