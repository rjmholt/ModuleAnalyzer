using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata
{
    public class EnumMemberMetadata : FieldMetadata
    {
        public EnumMemberMetadata(
            string name)
            : base(
                name,
                ProtectionLevel.Public,
                isStatic: true)
        {
        }
    }
}