namespace MetadataAnalysis.Metadata.Signatures
{
    /// <summary>
    /// A type signature as used in a method.
    /// </summary>
    public interface ITypeSignature
    {
        /// <summary>
        /// The type underlying the signature.
        /// </summary>
        TypeMetadata Type { get; }
    }

    /// <summary>
    /// A simple type signature, where the type is passed according to its own passing convention.
    /// </summary>
    public class SimpleTypeSignature : ITypeSignature
    {
        /// <summary>
        /// Create a simple type signature around a type.
        /// </summary>
        /// <param name="typeMetadata">The type that occurs in the signature.</param>
        public SimpleTypeSignature(TypeMetadata typeMetadata)
        {
            Type = typeMetadata;
        }

        /// <summary>
        /// The type passed in the type signature.
        /// </summary>
        public TypeMetadata Type { get; }
    }

    /// <summary>
    /// A type signature where the type is passed by reference.
    /// </summary>
    public class ByRefTypeSignature : ITypeSignature
    {
        /// <summary>
        /// Create a pass-by-reference type signature.
        /// </summary>
        /// <param name="typeMetadata">The type being passed by reference.</param>
        public ByRefTypeSignature(TypeMetadata typeMetadata)
        {
            Type = typeMetadata;
        }

        /// <summary>
        /// The type in the signature being passed by reference.
        /// </summary>
        public TypeMetadata Type { get; }
    }

    /// <summary>
    /// A type signature in which the type is passed out by reference.
    /// </summary>
    public class OutTypeSignature : ITypeSignature
    {
        /// <summary>
        /// Create a pass-out type signature.
        /// </summary>
        /// <param name="typeMetadata">The type being passed out of the signature.</param>
        public OutTypeSignature(TypeMetadata typeMetadata)
        {
            Type = typeMetadata;
        }

        /// <summary>
        /// The type being passed out of the signature.
        /// </summary>
        public TypeMetadata Type { get; }
    }
}