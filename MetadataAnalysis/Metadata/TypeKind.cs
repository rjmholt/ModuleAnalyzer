namespace MetadataAnalysis.Metadata
{
    /// <summary>
    /// Represents the kind of .NET base type a type metadata object refers to.
    /// </summary>
    public enum TypeKind
    {
        Class,
        Struct,
        Enum,
        ByReferenceType,
        ArrayType,
    }
}