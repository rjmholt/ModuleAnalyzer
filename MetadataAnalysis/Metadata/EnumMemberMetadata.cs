using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata
{
    public class EnumMemberMetadata : FieldMetadata
    {
        public EnumMemberMetadata(
            string name,
            IImmutableList<CustomAttributeMetadata> customAttributes = null)
            : base(name, ProtectionLevel.Public, isStatic: true, customAttributes: customAttributes)
        {
        }
    }
}