using System;
using System.Reflection.Metadata;

namespace MetadataAnalysis.Metadata.TypeProviders
{
    public class CustomAttributeTypeMetadataProvider : ICustomAttributeTypeProvider<TypeMetadata>
    {
        private readonly MetadataAnalyzer _metadataAnalyzer;

        private readonly SimpleTypeMetadataProvider _simpleTypeProvider;

        public CustomAttributeTypeMetadataProvider(MetadataAnalyzer metadataAnalyzer)
        {
            _metadataAnalyzer = metadataAnalyzer;
            _simpleTypeProvider = new SimpleTypeMetadataProvider(metadataAnalyzer);
        }

        public TypeMetadata GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            return _simpleTypeProvider.GetPrimitiveType(typeCode);
        }

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