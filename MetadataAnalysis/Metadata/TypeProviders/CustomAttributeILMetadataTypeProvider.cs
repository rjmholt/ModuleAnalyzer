using System.Reflection.Metadata;
using MetadataAnalysis.Metadata.ILParse;

namespace MetadataAnalysis.Metadata.TypeProviders
{
    public class CustomAttributeILMetadataTypeProvider : ICustomAttributeTypeProvider<TypeDefinitionILMetadata>
    {
        ISimpleTypeProvider<TypeDefinitionILMetadata> _simpleTypeProvider;
        ISZArrayTypeProvider<TypeDefinitionILMetadata> _szArrayTypeProvider;

        public CustomAttributeILMetadataTypeProvider(
            ISimpleTypeProvider<TypeDefinitionILMetadata> simpleTypeProvider,
            ISZArrayTypeProvider<TypeDefinitionILMetadata> szArrayTypeProvider
        )
        {
            _simpleTypeProvider = simpleTypeProvider;
            _szArrayTypeProvider = szArrayTypeProvider;
        }

        public TypeDefinitionILMetadata GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            return _simpleTypeProvider.GetPrimitiveType(typeCode);
        }

        public TypeDefinitionILMetadata GetSystemType()
        {
            throw new System.NotImplementedException();
        }

        public TypeDefinitionILMetadata GetSZArrayType(TypeDefinitionILMetadata elementType)
        {
            return _szArrayTypeProvider.GetSZArrayType(elementType);
        }

        public TypeDefinitionILMetadata GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
        {
            return _simpleTypeProvider.GetTypeFromDefinition(reader, handle, rawTypeKind);
        }

        public TypeDefinitionILMetadata GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
        {
            return _simpleTypeProvider.GetTypeFromReference(reader, handle, rawTypeKind);
        }

        public TypeDefinitionILMetadata GetTypeFromSerializedName(string name)
        {
            throw new System.NotImplementedException();
        }

        public PrimitiveTypeCode GetUnderlyingEnumType(TypeDefinitionILMetadata type)
        {
            throw new System.NotImplementedException();
        }

        public bool IsSystemType(TypeDefinitionILMetadata type)
        {
            throw new System.NotImplementedException();
        }
    }
}