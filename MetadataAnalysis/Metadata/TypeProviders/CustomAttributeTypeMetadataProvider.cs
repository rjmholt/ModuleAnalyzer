using System;
using System.Reflection.Metadata;

namespace MetadataAnalysis.Metadata.TypeProviders
{
    /// <summary>
    /// Provider for custom attribute types allowing reconstruction from custom attribute IL metadata.
    /// </summary>
    /// <typeparam name="TypeMetadata"></typeparam>
    public class CustomAttributeTypeMetadataProvider : ICustomAttributeTypeProvider<TypeMetadata>
    {
        /// <summary>
        /// The metadata analyzer to query metadata information from when required.
        /// </summary>
        private readonly MetadataAnalyzer _metadataAnalyzer;

        /// <summary>
        /// A simple type provider to provide simple type metadata.
        /// </summary>
        private readonly SimpleTypeMetadataProvider _simpleTypeProvider;

        /// <summary>
        /// Create a new custom attribute metadata provider using a metadata analyzer.
        /// </summary>
        /// <param name="metadataAnalyzer">The metadata analyzer to pull metadata information from.</param>
        public CustomAttributeTypeMetadataProvider(MetadataAnalyzer metadataAnalyzer)
        {
            _metadataAnalyzer = metadataAnalyzer;
            _simpleTypeProvider = new SimpleTypeMetadataProvider(metadataAnalyzer);
        }

        /// <summary>
        /// Get the type metadata from a primitive type given its type code.
        /// </summary>
        /// <param name="typeCode">The primitive type code to get metadata for.</param>
        /// <returns>A full metadata object representing the given primitive type.</returns>
        public TypeMetadata GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            return _simpleTypeProvider.GetPrimitiveType(typeCode);
        }

        /// <summary>
        /// Get a metadata object representing System.Type.
        /// </summary>
        /// <returns>A metadata object describing the System.Type type.</returns>
        public TypeMetadata GetSystemType()
        {
            return LoadedTypes.TypeTypeMetadata;
        }

        public TypeMetadata GetSZArrayType(TypeMetadata elementType)
        {
            if (LoadedTypes.IsTypeLoaded(elementType.FullName))
            {
                return LoadedTypes.GetByName(elementType.FullName + "[]");
            }

            throw new NotImplementedException();
        }

        public TypeMetadata GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
        {
            return _simpleTypeProvider.GetTypeFromDefinition(reader, handle, rawTypeKind);
        }

        public TypeMetadata GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
        {
            return _simpleTypeProvider.GetTypeFromReference(reader, handle, rawTypeKind);
        }

        public TypeMetadata GetTypeFromSerializedName(string name)
        {
            if (!LoadedTypes.TryFindByName(name, out TypeMetadata typeMetadata))
            {
                throw new Exception($"Cannot find type: '{name}'");
            }

            return typeMetadata;
        }

        public PrimitiveTypeCode GetUnderlyingEnumType(TypeMetadata type)
        {
            return ((EnumMetadata)type).UnderlyingEnumType;
        }

        public bool IsSystemType(TypeMetadata type)
        {
            return type == LoadedTypes.TypeTypeMetadata;
        }
    }
}