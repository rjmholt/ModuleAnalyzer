using System.Reflection.Metadata;
using MetadataAnalysis.Metadata.ILParse;

namespace MetadataAnalysis.Metadata.TypeProviders
{
    public class SimpleILMetadataTypeProvider : ISimpleTypeProvider<TypeILMetadata>
    {
        public TypeILMetadata GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            throw new System.NotImplementedException();
        }

        public TypeILMetadata GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
        {
            throw new System.NotImplementedException();
        }

        public TypeILMetadata GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
        {
            throw new System.NotImplementedException();
        }
    }
}