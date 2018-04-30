using MetadataAnalysis.Metadata.Interface;

namespace MetadataAnalysis.Metadata.ILParse
{
    public abstract class ILMemberMetadata : IMemberMetadata
    {
        protected ILMemberMetadata(string name, ProtectionLevel protectionLevel, bool isStatic)
        {
            Name = name;
            ProtectionLevel = protectionLevel;
            IsStatic = isStatic;
        }

        public string Name { get; }

        public ProtectionLevel ProtectionLevel { get; }

        public bool IsStatic { get; }
    }
}