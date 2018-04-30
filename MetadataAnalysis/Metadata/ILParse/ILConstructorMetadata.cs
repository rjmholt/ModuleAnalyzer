using MetadataAnalysis.Metadata.Interface;

namespace MetadataAnalysis.Metadata.ILParse
{
    public class ILConstructorMetadata : ILMethodMetadata, IConstructorMetadata
    {
        public const string CTOR_METHOD_NAME = "ctor";

        public ILConstructorMetadata(ProtectionLevel protectionLevel, bool isStatic)
            : base(CTOR_METHOD_NAME, protectionLevel, isStatic)
        {
        }
    }
}