using Microsoft.CodeAnalysis;

namespace MetadataHydrator
{
    public interface IFieldMetadata : IMemberMetadata
    {
        ITypeReferenceMetadata Type { get; }

        Accessibility Accessibility { get; }

        bool IsStatic { get; }
    }
}