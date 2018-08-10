using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using MetadataAnalysis.Interface;
using MetadataAnalysis.Lazy.Collections;

namespace MetadataAnalysis.Lazy
{
    public class LazyCustomAttributeMetadataList : LazyReadOnlyList<ICustomAttributeMetadata>
    {
        private readonly CustomAttributeHandleCollection _customAttributeHandles;

        private readonly MetadataReader _mdReader;

        public LazyCustomAttributeMetadataList(CustomAttributeHandleCollection customAttributeHandleCollection, MetadataReader mdReader)
        {
            _customAttributeHandles = customAttributeHandleCollection;
            _mdReader = mdReader;
        }

        protected override IEnumerable<ICustomAttributeMetadata> Generate()
        {
            foreach (CustomAttributeHandle caHandle in _customAttributeHandles)
            {
                yield return new LazyCustomAttributeMetadata(caHandle, _mdReader);
            }
        }
    }
}