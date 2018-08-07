namespace MetadataAnalysis.Metadata
{
    public class IndexerMetadata : MemberMetadata
    {
        public IndexerMetadata(
            ProtectionLevel protectionLevel,
            bool isStatic)
            : base(
                "Item",
                protectionLevel,
                isStatic)
        {
        }

        public TypeMetadata Type { get; internal set; }

        public TypeMetadata IndexType { get; internal set; }

        public IndexerGetterMetadata Getter { get; internal set; }

        public IndexerSetterMetadata Setter { get; internal set; }
    }

    public class IndexerGetterMetadata : MethodMetadata
    {
        private const string IndexerGetterName = "get_Item";

        public IndexerGetterMetadata(
            ProtectionLevel protectionLevel,
            bool isStatic = false)
            : base(
                IndexerGetterName,
                protectionLevel,
                isStatic)
        {
        }
    }

    public class IndexerSetterMetadata : MethodMetadata
    {
        private const string IndexerSetterName = "set_Item";

        public IndexerSetterMetadata(
            ProtectionLevel protectionLevel,
            bool isStatic = false)
            : base(
                IndexerSetterName,
                protectionLevel,
                isStatic)
        {
        }
    }
}