namespace MetadataHydrator.Interface
{
    public interface ITypeMetadata
    {
        string Name { get; }

        string Namespace { get; }

        string FullName { get; }

        TypeKind TypeKind { get; }

        ProtectionLevel ProtectionLevel { get; }

        ITypeMetadata BaseType { get; }

        IReadOnlyDictionary<string, IFieldMetadata> Fields { get; }

        IReadOnlyDictionary<string, IReadOnlyCollection<IPropertyMetadata>> Properties { get; }

        IReadOnlyDictionary<string, IReadOnlyCollection<IMethodMetadata>> Methods { get; }

        IReadOnlyDictionary<string, ITypeMetadata> NestedTypes { get; }
    }
}