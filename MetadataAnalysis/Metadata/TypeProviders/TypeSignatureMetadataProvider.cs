using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace MetadataAnalysis.Metadata.TypeProviders
{
    public class TypeSignatureMetadataProvider : ISignatureTypeProvider<TypeSignatureMetadata, TypeMetadataGenericContext>
    {
        private MetadataAnalyzer _metadataAnalyzer;

        public TypeSignatureMetadataProvider(MetadataAnalyzer metadataAnalyzer)
        {
            _metadataAnalyzer = metadataAnalyzer;
        }

        public TypeSignatureMetadata GetArrayType(TypeSignatureMetadata elementType, ArrayShape shape)
        {
            throw new NotImplementedException();
        }

        public TypeSignatureMetadata GetByReferenceType(TypeSignatureMetadata elementType)
        {
            throw new NotImplementedException();
        }

        public TypeSignatureMetadata GetFunctionPointerType(MethodSignature<TypeSignatureMetadata> signature)
        {
            throw new NotImplementedException();
        }

        public TypeSignatureMetadata GetGenericInstantiation(TypeSignatureMetadata genericType, ImmutableArray<TypeSignatureMetadata> typeArguments)
        {
            throw new NotImplementedException();
        }

        public TypeSignatureMetadata GetGenericMethodParameter(TypeMetadataGenericContext genericContext, int index)
        {
            throw new NotImplementedException();
        }

        public TypeSignatureMetadata GetGenericTypeParameter(TypeMetadataGenericContext genericContext, int index)
        {
            throw new NotImplementedException();
        }

        public TypeSignatureMetadata GetModifiedType(TypeSignatureMetadata modifier, TypeSignatureMetadata unmodifiedType, bool isRequired)
        {
            throw new NotImplementedException();
        }

        public TypeSignatureMetadata GetPinnedType(TypeSignatureMetadata elementType)
        {
            throw new NotImplementedException();
        }

        public TypeSignatureMetadata GetPointerType(TypeSignatureMetadata elementType)
        {
            throw new NotImplementedException();
        }

        public TypeSignatureMetadata GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            switch (typeCode)
            {
                case PrimitiveTypeCode.Boolean:
                    return LoadedTypes.FromType(typeof(Boolean));

                case PrimitiveTypeCode.Byte:
                    return LoadedTypes.FromType(typeof(Byte));

                case PrimitiveTypeCode.Char:
                    return LoadedTypes.FromType(typeof(Char));

                case PrimitiveTypeCode.Double:
                    return LoadedTypes.FromType(typeof(Double));

                case PrimitiveTypeCode.Int16:
                    return LoadedTypes.FromType(typeof(Int16));

                case PrimitiveTypeCode.Int32:
                    return LoadedTypes.FromType(typeof(Int32));

                case PrimitiveTypeCode.Int64:
                    return LoadedTypes.FromType(typeof(Int64));

                case PrimitiveTypeCode.IntPtr:
                    return LoadedTypes.FromType(typeof(IntPtr));

                case PrimitiveTypeCode.Object:
                    return LoadedTypes.FromType(typeof(Object));

                case PrimitiveTypeCode.SByte:
                    return LoadedTypes.FromType(typeof(SByte));

                case PrimitiveTypeCode.Single:
                    return LoadedTypes.FromType(typeof(Single));

                case PrimitiveTypeCode.String:
                    return LoadedTypes.FromType(typeof(String));

                case PrimitiveTypeCode.TypedReference:
                    return LoadedTypes.FromType(typeof(TypedReference));

                case PrimitiveTypeCode.UInt16:
                    return LoadedTypes.FromType(typeof(UInt16));

                case PrimitiveTypeCode.UInt32:
                    return LoadedTypes.FromType(typeof(UInt32));

                case PrimitiveTypeCode.UInt64:
                    return LoadedTypes.FromType(typeof(UInt64));

                case PrimitiveTypeCode.UIntPtr:
                    return LoadedTypes.FromType(typeof(UIntPtr));

                case PrimitiveTypeCode.Void:
                    return LoadedTypes.FromType(typeof(void));

                default:
                    throw new Exception(String.Format("Unknown typecode: '{0}'", typeCode));
            }
        }

        public TypeSignatureMetadata GetSZArrayType(TypeSignatureMetadata elementType)
        {
            throw new NotImplementedException();
        }

        public TypeSignatureMetadata GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
        {
            TypeDefinition typeDef = reader.GetTypeDefinition(handle);
            // TODO: Make this method accept a metadata reader
            return _metadataAnalyzer.ReadTypeMetadata(typeDef);
        }

        public TypeSignatureMetadata GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
        {
            TypeReference typeRef = reader.GetTypeReference(handle);
            _metadataAnalyzer.TryLookupTypeReference(typeRef, out TypeMetadata typeMetadata);
            return typeMetadata;
        }

        public TypeSignatureMetadata GetTypeFromSpecification(MetadataReader reader, TypeMetadataGenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
        {
            throw new NotImplementedException();
        }
    }
}