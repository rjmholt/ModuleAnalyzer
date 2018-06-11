
using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public class FieldMetadata : MemberMetadata
    {
        public FieldMetadata(
            string name,
            ProtectionLevel protectionLevel,
            bool isStatic = false)
            : base(name, protectionLevel, isStatic)
        {
        }

        public TypeMetadata Type { get; internal set; }
    }
}