using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator.Lazy.Metadata
{
    internal class LazyFieldMetadata : LazyMemberMetadata, IFieldMetadata
    {
        private readonly FieldDefinition _fieldDefinition;

        private ITypeReferenceMetadata _type;

        private readonly Lazy<IReadOnlyCollection<ICustomAttributeMetadata>> _customAttributes;

        public LazyFieldMetadata(
            string name,
            Accessibility accessibility,
            FieldDefinition fieldDefinition,
            LazyAssemblyHydrator assemblyHydrator)
            : base(name, assemblyHydrator)
        {
            _fieldDefinition = fieldDefinition;
            Accessibility = accessibility;

            _customAttributes = new Lazy<IReadOnlyCollection<ICustomAttributeMetadata>>(() => _assemblyHydrator.ReadCustomAttributes(_fieldDefinition.GetCustomAttributes()));
        }

        public Accessibility Accessibility { get; }

        public bool IsStatic { get => _fieldDefinition.Attributes.HasFlag(FieldAttributes.Static); }


        public ITypeReferenceMetadata Type
        {
            get
            {
                if (_type == null)
                {
                    ResolveSignature();
                }
                return _type;
            }
        }

        public override IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes
        {
            get => _customAttributes.Value;
        }

        protected override void ResolveSignature()
        {
            _type = _assemblyHydrator.ResolveFieldType(_fieldDefinition);
        }
    }
}