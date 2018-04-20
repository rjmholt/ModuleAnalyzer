using System.Reflection.Metadata;
using MetadataAnalysis.Metadata.ILParse;

namespace MetadataAnalysis.Metadata.TypeProviders
{
    public class SZArrayILMetadataTypeProvider : ISZArrayTypeProvider<TypeILMetadata>
    {
        public TypeILMetadata GetSZArrayType(TypeILMetadata elementType)
        {
            throw new System.NotImplementedException();
        }
    }
}