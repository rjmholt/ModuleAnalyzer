
using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata
{
    public class FieldMetadata : MemberMetadata
    {
        public FieldMetadata(
            string name,
            ProtectionLevel protectionLevel,
            bool isStatic = false,
            IImmutableList<GenericParameterMetadata> genericParameters = null,
            IImmutableList<CustomAttributeMetadata> customAttributes = null)
            : base(name, protectionLevel, isStatic, genericParameters, customAttributes)
        {
        }

        public TypeMetadata Type { get; internal set; }
    }
}