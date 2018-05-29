
using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata
{
    public class MethodMetadata : MemberMetadata
    {
        public MethodMetadata(
            string name,
            ProtectionLevel protectionLevel,
            bool isStatic = false,
            IImmutableList<GenericParameterMetadata> genericParameters = null,
            IImmutableList<CustomAttributeMetadata> customAttributes = null)
            : base(name, protectionLevel, isStatic, genericParameters, customAttributes)
        {
        }
    }
}