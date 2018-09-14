using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace MetadataHydrator.Lazy.Metadata
{
    internal class LazyPropertyMetadata : LazyMemberMetadata, IPropertyMetadata
    {
        private readonly PropertyDefinition _propertyDefinition;

        private readonly Lazy<IReadOnlyCollection<ICustomAttributeMetadata>> _customAttributes;

        public LazyPropertyMetadata(string name, PropertyDefinition propertyDefinition, LazyAssemblyHydrator assemblyHydrator)
            : base(name, assemblyHydrator)
        {
            _customAttributes = new Lazy<IReadOnlyCollection<ICustomAttributeMetadata>>(() => _assemblyHydrator.ReadCustomAttributes(_propertyDefinition.GetCustomAttributes()));
        }

        public ITypeDefinitionMetadata Type => throw new System.NotImplementedException();

        public override IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes => _customAttributes.Value;

        protected override void ResolveSignature()
        {
            throw new System.NotImplementedException();
        }
    }
}