using System;
using System.Reflection.Metadata;
using MetadataHydrator.Lazy.LoadedTypes;

namespace MetadataHydrator.Lazy.SignatureDecoding
{
    /// <summary>
    /// Provider for custom attribute types allowing reconstruction from custom attribute IL metadata.
    /// </summary>
    /// <typeparam name="TypeMetadata"></typeparam>
    public class CustomAttributeTypeMetadataProvider : ICustomAttributeTypeProvider<ITypeReferenceMetadata>
    {
        /// <summary>
        /// A simple type provider to provide simple type metadata.
        /// </summary>
        private readonly ISimpleTypeProvider<ITypeReferenceMetadata> _simpleTypeProvider;

        /// <summary>
        /// Create a new custom attribute metadata provider using a metadata analyzer.
        /// </summary>
        /// <param name="metadataAnalyzer">The metadata analyzer to pull metadata information from.</param>
        public CustomAttributeTypeMetadataProvider(LoadedTypeResolver loadedTypeResolver)
        {
            _simpleTypeProvider = new SimpleTypeMetadataProvider(loadedTypeResolver);
        }

        /// <summary>
        /// Get the type metadata from a primitive type given its type code.
        /// </summary>
        /// <param name="typeCode">The primitive type code to get metadata for.</param>
        /// <returns>A full metadata object representing the given primitive type.</returns>
        public ITypeReferenceMetadata GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            return _simpleTypeProvider.GetPrimitiveType(typeCode);
        }

        /// <summary>
        /// Get a metadata object representing System.Type.
        /// </summary>
        /// <returns>A metadata object describing the System.Type type.</returns>
        public ITypeReferenceMetadata GetSystemType()
        {
            throw new NotImplementedException();
        }

        public ITypeReferenceMetadata GetSZArrayType(ITypeReferenceMetadata elementType)
        {
            throw new NotImplementedException();
        }

        public ITypeReferenceMetadata GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
        {
            return _simpleTypeProvider.GetTypeFromDefinition(reader, handle, rawTypeKind);
        }

        public ITypeReferenceMetadata GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
        {
            return _simpleTypeProvider.GetTypeFromReference(reader, handle, rawTypeKind);
        }

        public ITypeReferenceMetadata GetTypeFromSerializedName(string name)
        {
            throw new NotImplementedException();
        }

        public PrimitiveTypeCode GetUnderlyingEnumType(ITypeReferenceMetadata type)
        {
            throw new NotImplementedException();
            // return ConvertEnumUnderlyingTypeCode();
        }

        public bool IsSystemType(ITypeReferenceMetadata type)
        {
            throw new NotImplementedException();
            // return type == LoadedTypes.TypeTypeMetadata;
        }

        public static PrimitiveTypeCode ConvertEnumUnderlyingTypeCode(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return PrimitiveTypeCode.Boolean;
                case TypeCode.Byte:
                    return PrimitiveTypeCode.Byte;
                case TypeCode.Char:
                    return PrimitiveTypeCode.Char;
                case TypeCode.Double:
                    return PrimitiveTypeCode.Double;
                case TypeCode.Int16:
                    return PrimitiveTypeCode.Int16;
                case TypeCode.Int32:
                    return PrimitiveTypeCode.Int32;
                case TypeCode.Int64:
                    return PrimitiveTypeCode.Int64;
                case TypeCode.SByte:
                    return PrimitiveTypeCode.SByte;
                case TypeCode.Single:
                    return PrimitiveTypeCode.Single;
                case TypeCode.UInt16:
                    return PrimitiveTypeCode.UInt16;
                case TypeCode.UInt32:
                    return PrimitiveTypeCode.UInt32;
                case TypeCode.UInt64:
                    return PrimitiveTypeCode.UInt64;
                default:
                    throw new Exception($"Unable to convert type code: '{typeCode}'");
            }
        }
    }
}