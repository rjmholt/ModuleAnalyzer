
using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public class MethodMetadata : MemberMetadata
    {
        public MethodMetadata(
            string name,
            ProtectionLevel protectionLevel,
            bool isStatic = false)
            : base(name, protectionLevel, isStatic)
        {
        }

        public TypeMetadata ReturnType { get; internal set; }

        public IImmutableList<TypeMetadata> ParameterTypes { get; internal set; }
    }
}