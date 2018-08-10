
using System.Collections.Generic;

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

        IReadOnlyList<IConstructorMetadata> Constructors { get; }

        IReadOnlyDictionary<string, IFieldMetadata> Fields { get; }

        IReadOnlyDictionary<string, IPropertyMetadata> Properties { get; }

        IReadOnlyDictionary<string, IReadOnlyList<IMethodMetadata>> Methods { get; }

        IReadOnlyList<IGenericParameterMetadata> GenericParameters { get; }

        IReadOnlyList<ICustomAttributeMetadata> CustomAttributes { get; }
    }
}