namespace MetadataAnalysis.Metadata
{
    public class ILConstructorMetadata : MethodMetadata
    {
        public const string CTOR_METHOD_NAME = "ctor";

        public ILConstructorMetadata(ProtectionLevel protectionLevel, bool isStatic)
            : base(CTOR_METHOD_NAME, protectionLevel, isStatic)
        {
        }
    }
}