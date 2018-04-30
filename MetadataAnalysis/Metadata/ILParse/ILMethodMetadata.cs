using MetadataAnalysis.Metadata.Interface;

namespace MetadataAnalysis.Metadata.ILParse
{
    public class ILMethodMetadata : ILMemberMetadata, IMethodMetadata
    {
        public ILMethodMetadata(string name, ProtectionLevel protectionLevel, bool isStatic) : base(name, protectionLevel, isStatic)
        {
        }
    }
}