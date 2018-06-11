using System.Collections.Generic;
using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public abstract class DefinedTypeMetadata : TypeMetadata
    {
        protected DefinedTypeMetadata(
            string name,
            string @namespace,
            string fullName,
            TypeKind typeKind,
            ProtectionLevel protectionLevel)
            : base(name, @namespace, fullName, typeKind, protectionLevel)
        {
        }

        public DefinedTypeMetadata DeclaringType { get; internal set; }

        public IImmutableDictionary<string, DefinedTypeMetadata> NestedTypes { get; internal set; }
    }
}