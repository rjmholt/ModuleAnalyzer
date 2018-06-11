namespace MetadataAnalysis.Metadata.Signature
{
    public abstract class SignatureTypeMetadata : TypeMetadata
    {
        public SignatureTypeMetadata(string prefix, TypeMetadata underlyingType)
            : base(
                prefix + underlyingType.Name,
                underlyingType.Namespace,
                prefix + underlyingType.FullName,
                TypeKind.ByReferenceType,
                underlyingType.ProtectionLevel)
        {
            BaseType = underlyingType.BaseType;
            UnderlyingType = underlyingType;
            Constructors = underlyingType.Constructors;
            Fields = underlyingType.Fields;
            Properties = underlyingType.Properties;
            Methods = underlyingType.Methods;
            GenericParameters = underlyingType.GenericParameters;
            CustomAttributes = underlyingType.CustomAttributes;
        }

        public TypeMetadata UnderlyingType { get; }
    }
}