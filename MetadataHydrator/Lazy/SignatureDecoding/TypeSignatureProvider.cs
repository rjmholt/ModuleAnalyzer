using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using MetadataHydrator.Lazy.LoadedTypes;

namespace MetadataHydrator.Lazy.SignatureDecoding
{
    public class TypeSignatureProvider : ISignatureTypeProvider<ITypeReferenceMetadata, LazyGenericContext>
    {
        private CustomAttributeTypeMetadataProvider _customAttributeMetadataProvider;

        public TypeSignatureProvider(LoadedAssemblyResolver loadedAssemblyResolver)
        {
            _customAttributeMetadataProvider = new CustomAttributeTypeMetadataProvider(loadedAssemblyResolver);
        }

        public ITypeReferenceMetadata GetArrayType(ITypeReferenceMetadata elementType, ArrayShape shape)
        {
            //return ArrayTypeMetadata.CreateArrayFromType(elementType, shape);
            throw new NotImplementedException();
        }

        public ITypeReferenceMetadata GetByReferenceType(ITypeReferenceMetadata elementType)
        {
            //return new ByRefTypeMetadata(elementType);
            throw new NotImplementedException();
        }

        public ITypeReferenceMetadata GetFunctionPointerType(MethodSignature<ITypeReferenceMetadata> signature)
        {
            throw new NotImplementedException();
        }

        public ITypeReferenceMetadata GetGenericInstantiation(ITypeReferenceMetadata genericType, ImmutableArray<ITypeReferenceMetadata> typeArguments)
        {
            throw new NotImplementedException();
        }

        public ITypeReferenceMetadata GetGenericMethodParameter(LazyGenericContext genericContext, int index)
        {
            //return genericContext.MethodParameters[index].Type;
            throw new NotImplementedException();
        }

        public ITypeReferenceMetadata GetGenericTypeParameter(LazyGenericContext genericContext, int index)
        {
            throw new NotImplementedException();
        }

        public ITypeReferenceMetadata GetModifiedType(ITypeReferenceMetadata modifier, ITypeReferenceMetadata unmodifiedType, bool isRequired)
        {
            // TODO: Take account of the modifier
            //return new VolatileTypeMetadata(unmodifiedType);
            throw new NotImplementedException();
        }

        public ITypeReferenceMetadata GetPinnedType(ITypeReferenceMetadata elementType)
        {
            //return new PinnedTypeMetadata(elementType);
            throw new NotImplementedException();
        }

        public ITypeReferenceMetadata GetPointerType(ITypeReferenceMetadata elementType)
        {
            //return new PointerTypeMetadata(elementType);
            throw new NotImplementedException();
        }

        public ITypeReferenceMetadata GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            return _customAttributeMetadataProvider.GetPrimitiveType(typeCode);
        }

        public ITypeReferenceMetadata GetSZArrayType(ITypeReferenceMetadata elementType)
        {
            //return ArrayTypeMetadata.CreateArrayFromType(elementType, ArrayTypeMetadata.s_szArrayShape);
            throw new NotImplementedException();
        }

        public ITypeReferenceMetadata GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
        {
            return _customAttributeMetadataProvider.GetTypeFromDefinition(reader, handle, rawTypeKind);
        }

        public ITypeReferenceMetadata GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
        {
            return _customAttributeMetadataProvider.GetTypeFromReference(reader, handle, rawTypeKind);
        }

        public ITypeReferenceMetadata GetTypeFromSpecification(MetadataReader reader, LazyGenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
        {
            throw new NotImplementedException();
        }
    }
}