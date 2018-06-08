using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using MetadataAnalysis.Metadata.Signature;

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
            return new ByRefTypeMetadata((NameableTypeMetadata)elementType);
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
            return new VolatileTypeMetadata((NameableTypeMetadata)unmodifiedType);
        }

        public TypeMetadata GetPinnedType(TypeMetadata elementType)
        {
            return new PinnedTypeMetadata((NameableTypeMetadata)elementType);
        }

        public TypeMetadata GetPointerType(TypeMetadata elementType)
        {
            return new PointerTypeMetadata((NameableTypeMetadata)elementType);
        }

        public TypeMetadata GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            return _customAttributeMetadataProvider.GetPrimitiveType(typeCode);
        }

        public TypeMetadata GetSZArrayType(TypeMetadata elementType)
        {
            throw new NotImplementedException();
        }

        public TypeMetadata GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
        {
            return _customAttributeMetadataProvider.GetTypeFromDefinition(reader, handle, rawTypeKind);
        }

        public TypeMetadata GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
        {
            return _customAttributeMetadataProvider.GetTypeFromReference(reader, handle, rawTypeKind);
        }

        public TypeMetadata GetTypeFromSpecification(MetadataReader reader, TypeMetadataGenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
        {
            throw new NotImplementedException();
        }
    }
}