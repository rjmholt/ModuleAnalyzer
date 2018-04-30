using System.Collections.Immutable;

namespace MetadataAnalysis.Metadata.Interface
{
    /// <summary>
    /// Describes the metadata of a type.
    /// </summary>
    public interface ITypeMetadata
    {
        /// <summary>
        /// The name of the type.
        /// </summary>
         string Name { get; }

        /// <summary>
        /// The namespace of the type.
        /// </summary>
         string Namespace { get; }

        /// <summary>
        /// The full, namespace-qualified name of the type.
        /// </summary>
         string FullName { get; }

        /// <summary>
        /// The kind of the type.
        /// </summary>
         TypeKind TypeKind { get; }

        /// <summary>
        /// The accessibility level of the type.
        /// </summary>
         ProtectionLevel ProtectionLevel { get; }

        /// <summary>
        /// The type this type inherits directly from, possibly null.
        /// </summary>
         ITypeMetadata BaseType { get; }

        /// <summary>
        /// The type this type is nested in, if any (null otherwise).
        /// </summary>
         ITypeMetadata DeclaringType { get; }

        /// <summary>
        /// Constructors for this type.
        /// </summary>
         IImmutableList<IConstructorMetadata> Constructors { get; }

        /// <summary>
        /// Fields on this type.
        /// </summary>
         IImmutableDictionary<string, IFieldMetadata> Fields { get; }

        /// <summary>
        /// Properties on this type.
        /// </summary>
         IImmutableDictionary<string, IPropertyMetadata> Properties { get; }

        /// <summary>
        /// Methods on this type.
        /// </summary>
         IImmutableDictionary<string, IImmutableList<IMethodMetadata>> Methods { get; }

        /// <summary>
        /// Types defined within this type.
        /// </summary>
         IImmutableDictionary<string, ITypeMetadata> NestedTypes { get; }
    }
}