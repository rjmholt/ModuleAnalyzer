using System;
using System.Reflection.Metadata;
using MetadataHydrator.Lazy.LoadedTypes;

namespace MetadataHydrator.Lazy.SignatureDecoding
{
    public class SimpleTypeMetadataProvider : ISimpleTypeProvider<ITypeReferenceMetadata>
    {
        private readonly LoadedAssemblyResolver _loadedAssemblyResolver;

        public SimpleTypeMetadataProvider(LoadedAssemblyResolver loadedAssemblyResolver)
        {
            _loadedAssemblyResolver = loadedAssemblyResolver;
        }

        public ITypeReferenceMetadata GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            switch (typeCode)
            {
                case PrimitiveTypeCode.Boolean:
                    return _loadedAssemblyResolver.ResolveType(typeof(Boolean));

                case PrimitiveTypeCode.Byte:
                    return _loadedAssemblyResolver.ResolveType(typeof(Byte));

                case PrimitiveTypeCode.Char:
                    return _loadedAssemblyResolver.ResolveType(typeof(Char));

                case PrimitiveTypeCode.Double:
                    return _loadedAssemblyResolver.ResolveType(typeof(Double));

                case PrimitiveTypeCode.Int16:
                    return _loadedAssemblyResolver.ResolveType(typeof(Int16));

                case PrimitiveTypeCode.Int32:
                    return _loadedAssemblyResolver.ResolveType(typeof(Int32));

                case PrimitiveTypeCode.Int64:
                    return _loadedAssemblyResolver.ResolveType(typeof(Int64));

                case PrimitiveTypeCode.IntPtr:
                    return _loadedAssemblyResolver.ResolveType(typeof(IntPtr));

                case PrimitiveTypeCode.Object:
                    return _loadedAssemblyResolver.ResolveType(typeof(Object));

                case PrimitiveTypeCode.SByte:
                    return _loadedAssemblyResolver.ResolveType(typeof(SByte));

                case PrimitiveTypeCode.Single:
                    return _loadedAssemblyResolver.ResolveType(typeof(Single));

                case PrimitiveTypeCode.String:
                    return _loadedAssemblyResolver.ResolveType(typeof(String));

                case PrimitiveTypeCode.TypedReference:
                    return _loadedAssemblyResolver.ResolveType(typeof(TypedReference));

                case PrimitiveTypeCode.UInt16:
                    return _loadedAssemblyResolver.ResolveType(typeof(UInt16));

                case PrimitiveTypeCode.UInt32:
                    return _loadedAssemblyResolver.ResolveType(typeof(UInt32));

                case PrimitiveTypeCode.UInt64:
                    return _loadedAssemblyResolver.ResolveType(typeof(UInt64));

                case PrimitiveTypeCode.UIntPtr:
                    return _loadedAssemblyResolver.ResolveType(typeof(UIntPtr));

                case PrimitiveTypeCode.Void:
                    return _loadedAssemblyResolver.ResolveType(typeof(void));

                default:
                    throw new Exception($"Unknown typecode: '{typeCode}'");
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