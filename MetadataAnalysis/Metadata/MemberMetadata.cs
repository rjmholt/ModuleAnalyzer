
using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public abstract class MemberMetadata
    {
        protected MemberMetadata(
            string name,
            ProtectionLevel protectionLevel,
            bool isStatic = false)
        {
            Name = name;
            ProtectionLevel = protectionLevel;
            IsStatic = isStatic;
        }

        public string Name { get; }

        public ProtectionLevel ProtectionLevel { get; }

        public IImmutableList<CustomAttributeMetadata> CustomAttributes { get; internal set; }

        public IImmutableList<GenericParameterMetadata> GenericParameters { get; internal set; }

        public bool IsStatic { get; }
    }
}