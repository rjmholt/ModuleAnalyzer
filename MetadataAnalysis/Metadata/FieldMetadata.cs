
namespace MetadataAnalysis.Metadata
{
    public class FieldMetadata
    {
        public FieldMetadata(
            TypeMetadata type,
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

        public TypeMetadata Type { get; }

        public string Name { get; }

        public ProtectionLevel ProtectionLevel { get; }

        public bool IsStatic { get; }
    }
}