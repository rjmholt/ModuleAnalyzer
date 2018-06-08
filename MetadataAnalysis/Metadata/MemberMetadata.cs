
using System.Collections.Immutable;
using MetadataAnalysis.Metadata.Generic;

namespace MetadataAnalysis.Metadata
{
    public abstract class MemberMetadata
    {
        protected MemberMetadata(
            string name,
            ProtectionLevel protectionLevel,
            bool isStatic = false, 
            IImmutableList<GenericParameterMetadata> genericParameters = null,
            IImmutableList<CustomAttributeMetadata> customAttributes = null)
        {
            Name = name;
            ProtectionLevel = protectionLevel;
            IsStatic = isStatic;
            GenericParameters = genericParameters ?? ImmutableArray<GenericParameterMetadata>.Empty;
            CustomAttributes = customAttributes ?? ImmutableArray<CustomAttributeMetadata>.Empty;
        }

        public string Name { get; }

        public ProtectionLevel ProtectionLevel { get; }

        public IImmutableList<CustomAttributeMetadata> CustomAttributes { get; }

        public IImmutableList<GenericParameterMetadata> GenericParameters { get; }

        public bool IsStatic { get; }
    }
}