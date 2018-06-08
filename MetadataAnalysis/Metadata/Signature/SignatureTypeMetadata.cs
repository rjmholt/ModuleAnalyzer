namespace MetadataAnalysis.Metadata.Signature
{
    public abstract class SignatureTypeMetadata : TypeMetadata
    {
        public SignatureTypeMetadata(string prefix, NameableTypeMetadata underlyingType)
            : base(
                prefix + underlyingType.Name,
                underlyingType.Namespace,
                TypeKind.ByReferenceType,
                underlyingType.ProtectionLevel,
                underlyingType.BaseType,
                underlyingType.Constructors,
                underlyingType.Fields,
                underlyingType.Properties,
                underlyingType.Methods,
                underlyingType.GenericParameters,
                underlyingType.CustomAttributes)
        {
            UnderlyingType = underlyingType;
        }

        public TypeMetadata UnderlyingType { get; }
    }
}