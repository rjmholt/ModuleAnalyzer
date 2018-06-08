using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public class PropertyMetadata : MemberMetadata
    {
        public PropertyMetadata(
            string name,
            TypeMetadata type,
            ProtectionLevel protectionLevel,
            PropertyGetterMetadata getter,
            PropertySetterMetadata setter,
            bool isStatic = false,
            IImmutableList<GenericParameterMetadata> genericParameters = null,
            IImmutableList<CustomAttributeMetadata> customAttributes = null)
            : base(name, protectionLevel, isStatic, genericParameters, customAttributes)
        {
            Type = type;
            Getter = getter;
            Setter = setter;
        }

        public TypeMetadata Type { get; internal set; }

        public PropertyGetterMetadata Getter { get; }

        public PropertySetterMetadata Setter { get; }
    }
}