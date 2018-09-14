using Microsoft.CodeAnalysis;

namespace MetadataHydrator
{
    public interface IFieldMetadata : IMemberMetadata
    {
        ITypeDefinitionMetadata Type { get; }

        Accessibility Accessibility { get; }

        bool IsStatic { get; }
    }
}