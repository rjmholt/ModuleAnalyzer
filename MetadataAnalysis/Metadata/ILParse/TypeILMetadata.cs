using System.Collections.Generic;
using System.Reflection.Metadata;

namespace MetadataAnalysis.Metadata.ILParse
{
    public class TypeILMetadata : ITypeMetadata
    {
        public static IEnumerable<TypeILMetadata> FromMetadataReader(MetadataReader mdReader)
        {
            yield break;
        }
        public string Name => throw new System.NotImplementedException();
    }
}