using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace MetadataAnalysis.Metadata.TypeProviders
{
    public class TypeSignatureProvider : ISignatureTypeProvider<TypeMetadata, TypeMetadataGenericContext>
    {
        private MetadataAnalyzer _metadataAnalyzer;

        private CustomAttributeTypeMetadataProvider _customAttributeMetadataProvider;

        public TypeSignatureProvider(MetadataAnalyzer metadataAnalyzer)
        {
            _metadataAnalyzer = metadataAnalyzer;
            _customAttributeMetadataProvider = new CustomAttributeTypeMetadataProvider(metadataAnalyzer);
        }

        public TypeMetadata GetArrayType(TypeMetadata elementType, ArrayShape shape)
        {
            throw new NotImplementedException();
        }

        public TypeMetadata GetByReferenceType(TypeMetadata elementType)
        {
            throw new NotImplementedException();
        }

        public TypeMetadata GetFunctionPointerType(MethodSignature<TypeMetadata> signature)
        {
            throw new NotImplementedException();
        }

        public TypeMetadata GetGenericInstantiation(TypeMetadata genericType, ImmutableArray<TypeMetadata> typeArguments)
        {
            throw new NotImplementedException();
        }

        public TypeMetadata GetGenericMethodParameter(TypeMetadataGenericContext genericContext, int index)
        {
            throw new NotImplementedException();
        }

        public TypeMetadata GetGenericTypeParameter(TypeMetadataGenericContext genericContext, int index)
        {
            throw new NotImplementedException();
        }

        public TypeMetadata GetModifiedType(TypeMetadata modifier, TypeMetadata unmodifiedType, bool isRequired)
        {
            throw new NotImplementedException();
        }

        public TypeMetadata GetPinnedType(TypeMetadata elementType)
        {
            throw new NotImplementedException();
        }

        public TypeMetadata GetPointerType(TypeMetadata elementType)
        {
            throw new NotImplementedException();
        }

        public TypeMetadata GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            throw new NotImplementedException();
        }

        public TypeMetadata GetSZArrayType(TypeMetadata elementType)
        {
            throw new NotImplementedException();
        }

        public TypeMetadata GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
        {
            throw new NotImplementedException();
        }

        public TypeMetadata GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
        {
            throw new NotImplementedException();
        }

        public TypeMetadata GetTypeFromSpecification(MetadataReader reader, TypeMetadataGenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
        {
            throw new NotImplementedException();
        }
    }
}