using System;
using System.Reflection.Metadata;
using MetadataHydrator.Lazy.LoadedTypes;

namespace MetadataHydrator.Lazy.SignatureDecoding
{
    public class SimpleTypeMetadataProvider : ISimpleTypeProvider<ITypeReferenceMetadata>
    {
        private readonly LoadedTypeResolver _loadedTypeResolver;

        public SimpleTypeMetadataProvider(LoadedTypeResolver loadedTypeResolver)
        {
            _loadedTypeResolver = loadedTypeResolver;
        }

        public ITypeReferenceMetadata GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            switch (typeCode)
            {
                case PrimitiveTypeCode.Boolean:
                    return _loadedTypeResolver.FromType(typeof(Boolean));

                case PrimitiveTypeCode.Byte:
                    return _loadedTypeResolver.FromType(typeof(Byte));

                case PrimitiveTypeCode.Char:
                    return _loadedTypeResolver.FromType(typeof(Char));

                case PrimitiveTypeCode.Double:
                    return _loadedTypeResolver.FromType(typeof(Double));

                case PrimitiveTypeCode.Int16:
                    return _loadedTypeResolver.FromType(typeof(Int16));

                case PrimitiveTypeCode.Int32:
                    return _loadedTypeResolver.FromType(typeof(Int32));

                case PrimitiveTypeCode.Int64:
                    return _loadedTypeResolver.FromType(typeof(Int64));

                case PrimitiveTypeCode.IntPtr:
                    return _loadedTypeResolver.FromType(typeof(IntPtr));

                case PrimitiveTypeCode.Object:
                    return _loadedTypeResolver.FromType(typeof(Object));

                case PrimitiveTypeCode.SByte:
                    return _loadedTypeResolver.FromType(typeof(SByte));

                case PrimitiveTypeCode.Single:
                    return _loadedTypeResolver.FromType(typeof(Single));

                case PrimitiveTypeCode.String:
                    return _loadedTypeResolver.FromType(typeof(String));

                case PrimitiveTypeCode.TypedReference:
                    return _loadedTypeResolver.FromType(typeof(TypedReference));

                case PrimitiveTypeCode.UInt16:
                    return _loadedTypeResolver.FromType(typeof(UInt16));

                case PrimitiveTypeCode.UInt32:
                    return _loadedTypeResolver.FromType(typeof(UInt32));

                case PrimitiveTypeCode.UInt64:
                    return _loadedTypeResolver.FromType(typeof(UInt64));

                case PrimitiveTypeCode.UIntPtr:
                    return _loadedTypeResolver.FromType(typeof(UIntPtr));

                case PrimitiveTypeCode.Void:
                    return _loadedTypeResolver.FromType(typeof(void));

                default:
                    throw new Exception(String.Format("Unknown typecode: '{0}'", typeCode));
            }
        }

        public ITypeReferenceMetadata GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
        {
            TypeDefinition typeDef = reader.GetTypeDefinition(handle);
            // TODO: Make this method accept a metadata reader
            throw new NotImplementedException();
        }

        public ITypeReferenceMetadata GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
        {
            TypeReference typeRef = reader.GetTypeReference(handle);

            throw new NotImplementedException();
        }
    }
}