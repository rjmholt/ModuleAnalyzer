
using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public class FieldMetadata : MemberMetadata
    {
        public new class Prototype : MemberMetadata.Prototype, IPrototype<FieldMetadata>
        {
            public FieldMetadata Get()
            {
                return null;
            }
        }

        public FieldMetadata(
            string name,
            ProtectionLevel protectionLevel,
            bool isStatic = false,
            IImmutableList<GenericParameterMetadata> genericParameters = null,
            IImmutableList<CustomAttributeMetadata> customAttributes = null)
            : base(name, protectionLevel, isStatic, genericParameters, customAttributes)
        {
        }

        public TypeMetadata Type { get; internal set; }
    }
}