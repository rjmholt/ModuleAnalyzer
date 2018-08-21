using Microsoft.CodeAnalysis;

namespace MetadataHydrator
{
    public interface IFieldMetadata : IMemberMetadata
    {
        ITypeMetadata Type { get; }

        Accessibility Accessibility { get; }

        bool IsStatic { get; }
    }
}