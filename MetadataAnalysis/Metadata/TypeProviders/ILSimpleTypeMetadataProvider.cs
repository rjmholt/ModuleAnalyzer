using System;
using System.Reflection.Metadata;

namespace MetadataAnalysis.Metadata.TypeProviders
{
    public class ILSimpleTypeMetadataProvider : ISimpleTypeProvider<TypeMetadata>
    {
        private MetadataAnalyzer _metadataAnalyzer;

        public ILSimpleTypeMetadataProvider(MetadataAnalyzer metadataAnalyzer)
        {
            _metadataAnalyzer = metadataAnalyzer;
        }

        public TypeMetadata GetPrimitiveType(PrimitiveTypeCode typeCode)
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

        public TypeMetadata GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
        {
            TypeDefinition typeDef = reader.GetTypeDefinition(handle);
            // TODO: Make this method accept a metadata reader
            return _metadataAnalyzer.ReadTypeMetadata(typeDef);
        }

        public TypeMetadata GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
        {
            TypeReference typeRef = reader.GetTypeReference(handle);
            _metadataAnalyzer.TryLookupTypeReference(typeRef, out TypeMetadata typeMetadata);
            return typeMetadata;
        }
    }
}