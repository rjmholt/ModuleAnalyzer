using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using MetadataAnalysis.Metadata.Array;
using MetadataAnalysis.Metadata.Generic;
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
            return new ByRefTypeMetadata(elementType);
        }

        public TypeMetadata GetFunctionPointerType(MethodSignature<TypeMetadata> signature)
        {
            throw new NotImplementedException();
        }

        public TypeMetadata GetGenericInstantiation(TypeMetadata genericType, ImmutableArray<TypeMetadata> typeArguments)
        {
            return _metadataAnalyzer.GetGenericInstantiation(genericType, typeArguments);
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
            // TODO: Take account of the modifier
            return new VolatileTypeMetadata(unmodifiedType);
        }

        public TypeMetadata GetPinnedType(TypeMetadata elementType)
        {
            return new PinnedTypeMetadata(elementType);
        }

        public TypeMetadata GetPointerType(TypeMetadata elementType)
        {
            return new PointerTypeMetadata(elementType);
        }

        public TypeMetadata GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            return _customAttributeMetadataProvider.GetPrimitiveType(typeCode);
        }

        public TypeMetadata GetSZArrayType(TypeMetadata elementType)
        {
            return ArrayTypeMetadata.CreateArrayFromType(elementType);
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