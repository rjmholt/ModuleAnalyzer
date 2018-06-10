
using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public class MethodMetadata : MemberMetadata
    {
        public new class Prototype
        {
            public override MemberMetadata Get()
            {
            }
        }

        public MethodMetadata(
            string name,
            ProtectionLevel protectionLevel,
            bool isStatic = false,
            IImmutableList<GenericParameterMetadata> genericParameters = null,
            IImmutableList<CustomAttributeMetadata> customAttributes = null)
            : base(name, protectionLevel, isStatic, genericParameters, customAttributes)
        {
        }

        public TypeMetadata ReturnType { get; internal set; }

        public IImmutableList<TypeMetadata> ParameterTypes { get; internal set; }
    }
}