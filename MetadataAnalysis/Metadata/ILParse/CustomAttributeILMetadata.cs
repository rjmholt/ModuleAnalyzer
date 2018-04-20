using System.Collections.Generic;
using System.Reflection.Metadata;

namespace MetadataAnalysis.Metadata.ILParse
{
    public class CustomAttributeILMetadata : ICustomAttributeMetadata
    {
        public static IEnumerable<CustomAttributeILMetadata> FromHandleCollectionWithMetadataReader(
            MetadataReader mdReader,
            CustomAttributeHandleCollection customAttributeHandles)
        {
            foreach (CustomAttributeHandle caHandle in customAttributeHandles)
            {
                if (!caHandle.IsNil)
                {
                    CustomAttribute customAttribute = mdReader.GetCustomAttribute(caHandle);
                }
            }

            yield break;
        }
    }
}