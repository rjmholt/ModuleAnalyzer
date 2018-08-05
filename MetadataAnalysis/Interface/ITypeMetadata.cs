using System.Collections.Immutable;

namespace MetadataAnalysis.Interface
{
    public interface ITypeMetadata
    {
        string Name { get; }

        string Namespace { get; }

        string FullName { get; }

        TypeKind TypeKind { get; }

        ProtectionLevel ProtectionLevel { get; }

        ITypeMetadata BaseType { get; }

        IImmutableList<IConstructorMetadata> Constructors { get; }

        IImmutableDictionary<string, IFieldMetadata> Fields { get; }

        IImmutableDictionary<string, IPropertyMetadata> Properties { get; }

        IImmutableDictionary<string, IImmutableList<IMethodMetadata>> Methods { get; }

        IImmutableList<IGenericParameterMetadata> GenericParameters { get; }

        IImmutableList<ICustomAttributeMetadata> CustomAttributes { get; }
    }
}