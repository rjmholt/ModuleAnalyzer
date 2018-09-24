using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator.Lazy.Metadata
{
    internal class LazyTypeDefinitionMetadata : ITypeDefinitionMetadata
    {
        private readonly TypeDefinition _typeDefinition;

        private readonly LazyAssemblyHydrator _assemblyHydrator;

        private readonly Lazy<ITypeReferenceMetadata> _baseType;

        private readonly Lazy<IReadOnlyDictionary<string, IFieldMetadata>> _fields;

        private readonly Lazy<IReadOnlyDictionary<string, IReadOnlyCollection<IPropertyMetadata>>> _properties;

        private readonly Lazy<IReadOnlyDictionary<string, IReadOnlyCollection<IMethodMetadata>>> _methods;

        private readonly Lazy<IReadOnlyDictionary<string, ITypeDefinitionMetadata>> _nestedTypes;

        private readonly Lazy<IReadOnlyCollection<ICustomAttributeMetadata>> _customAttributes;

        private readonly Lazy<IReadOnlyCollection<IGenericParameterMetadata>> _genericParameters;

        public LazyTypeDefinitionMetadata(
            TypeDefinition typeDefintion,
            LazyAssemblyDefinitionMetadata assembly,
            string name,
            string @namespace,
            string fullName,
            Accessibility accessibility,
            LazyAssemblyHydrator assemblyHydrator)
        {
            _typeDefinition = typeDefintion;
            _assemblyHydrator = assemblyHydrator;

            Assembly = assembly;
            Name = name;
            Namespace = @namespace;
            FullName = fullName;
            Accessibility = accessibility;

            _baseType = new Lazy<ITypeReferenceMetadata>(() => _assemblyHydrator.ReadTypeFromHandle(_typeDefinition.BaseType));
            _fields = new Lazy<IReadOnlyDictionary<string, IFieldMetadata>>(() => _assemblyHydrator.ReadFields(_typeDefinition.GetFields()));
            _properties = new Lazy<IReadOnlyDictionary<string, IReadOnlyCollection<IPropertyMetadata>>>(() => _assemblyHydrator.ReadProperties(_typeDefinition.GetProperties()));
            _methods = new Lazy<IReadOnlyDictionary<string, IReadOnlyCollection<IMethodMetadata>>>(() => _assemblyHydrator.ReadMethods(_typeDefinition.GetMethods()));
            _nestedTypes = new Lazy<IReadOnlyDictionary<string, ITypeDefinitionMetadata>>(() => _assemblyHydrator.ReadTypeDefinitions(_typeDefinition.GetNestedTypes(), enclosingType: this));
            _customAttributes = new Lazy<IReadOnlyCollection<ICustomAttributeMetadata>>(() => _assemblyHydrator.ReadCustomAttributes(_typeDefinition.GetCustomAttributes()));
            _genericParameters = new Lazy<IReadOnlyCollection<IGenericParameterMetadata>>(() => _assemblyHydrator.ReadGenericParameters(_typeDefinition.GetGenericParameters()));
        }

        public string Name { get; }

        public string Namespace { get; }

        public string FullName { get; }

        public Accessibility Accessibility { get; }

        public ITypeReferenceMetadata BaseType
        {
            get => _baseType.Value;
        }

        public IReadOnlyDictionary<string, IFieldMetadata> Fields
        {
            get => _fields.Value;
        }

        public IReadOnlyDictionary<string, IReadOnlyCollection<IPropertyMetadata>> Properties
        {
            get => _properties.Value;
        }

        public IReadOnlyDictionary<string, IReadOnlyCollection<IMethodMetadata>> Methods
        {
            get => _methods.Value;
        }

        public IReadOnlyDictionary<string, ITypeDefinitionMetadata> NestedTypes
        {
            get => _nestedTypes.Value;
        }

        public IAssemblyDefinitionMetadata Assembly { get; }

        public IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes
        {
            get => _customAttributes.Value;
        }

        public IReadOnlyCollection<IGenericParameterMetadata> GenericParameters
        {
            get => _genericParameters.Value;
        }
    }
}