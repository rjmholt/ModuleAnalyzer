using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata
{
    public class ConstructorMetadata : MethodMetadata
    {
        public const string CTOR_METHOD_NAME = "ctor";

        public ConstructorMetadata(
            ProtectionLevel protectionLevel,
            bool isStatic = false,
            IImmutableList<CustomAttributeMetadata> customAttributes = null)
            : base(CTOR_METHOD_NAME, protectionLevel, isStatic)
        {
        }
    }
}