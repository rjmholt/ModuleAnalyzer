using MetadataAnalysis.Metadata.Interface;

namespace MetadataAnalysis.Metadata.ILParse
{
    public class ILFieldMetadata : IFieldMetadata
    {
        public ILFieldMetadata(
            ITypeMetadata type,
            string name,
            ProtectionLevel protectionLevel,
            bool isStatic
        )
        {
            Type = type;
            Name = name;
            ProtectionLevel = protectionLevel;
            IsStatic = isStatic;
        }

        public ITypeMetadata Type { get; }

        public string Name { get; }

        public ProtectionLevel ProtectionLevel { get; }

        public bool IsStatic { get; }
    }
}