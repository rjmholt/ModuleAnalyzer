
namespace MetadataAnalysis.Metadata
{
    public abstract class MemberMetadata
    {
        protected MemberMetadata(string name, ProtectionLevel protectionLevel, bool isStatic)
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