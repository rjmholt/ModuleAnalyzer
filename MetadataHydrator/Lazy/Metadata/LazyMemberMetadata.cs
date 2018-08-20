using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator.Lazy.Metadata
{
    public abstract class LazyMemberMetadata : IMemberMetadata
    {
        protected LazyMemberMetadata(string name, Accessibility accessibility, bool isStatic)
        {
            Name = name;
            Accessibility = accessibility;
            IsStatic = isStatic;
        }

        public string Name { get; }

        public Accessibility Accessibility { get; }

        public bool IsStatic { get; }

        public abstract IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes { get; }

        public abstract IReadOnlyCollection<IGenericParameterMetadata> GenericParameters { get; }
    }
}