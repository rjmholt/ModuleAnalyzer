using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public class PropertyMetadata : MemberMetadata
    {
        public PropertyMetadata(
            string name,
            ProtectionLevel protectionLevel,
            bool isStatic = false)
            : base(name, protectionLevel, isStatic)
        {
        }

        public TypeMetadata Type { get; internal set; }

        public PropertyGetterMetadata Getter { get; internal set; }

        public PropertySetterMetadata Setter { get; internal set; }
    }
}