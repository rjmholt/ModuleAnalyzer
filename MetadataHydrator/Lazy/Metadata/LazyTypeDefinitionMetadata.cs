using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator.Lazy.Metadata
{
    internal class LazyTypeDefinitionMetadata : ITypeMetadata
    {
        private readonly TypeDefinition _typeDefinition;

        private readonly LazyAssemblyHydrator _assemblyHydrator;

        private ITypeMetadata _baseType;

        private IReadOnlyDictionary<string, IFieldMetadata> _fields;

        private IReadOnlyDictionary<string, IReadOnlyCollection<IPropertyMetadata>> _properties;

        private IReadOnlyDictionary<string, IReadOnlyCollection<IMethodMetadata>> _methods;

        private IReadOnlyDictionary<string, ITypeMetadata> _nestedTypes;

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
        }

        public string Name { get; }

        public string Namespace { get; }

        public string FullName { get; }

        public Accessibility Accessibility { get; }

        public ITypeMetadata BaseType
        {
            get
            {
                if (_baseType == null)
                {
                    _baseType = _assemblyHydrator.ReadBaseType(_typeDefinition);
                }
                return _baseType;
            }
        }

        public IReadOnlyDictionary<string, IFieldMetadata> Fields
        {
            get
            {
                if (_fields == null)
                {
                    _fields = _assemblyHydrator.ReadTypeFields(_typeDefinition);
                }
                return _fields;
            }
        }

        public IReadOnlyDictionary<string, IReadOnlyCollection<IPropertyMetadata>> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = _assemblyHydrator.ReadTypeProperties(_typeDefinition);
                }
                return _properties;
            }
        }

        public IReadOnlyDictionary<string, IReadOnlyCollection<IMethodMetadata>> Methods
        {
            get
            {
                if (_methods == null)
                {
                    _methods = _assemblyHydrator.ReadTypeMethods(_typeDefinition);
                }
                return _methods;
            }
        }

        public IReadOnlyDictionary<string, ITypeMetadata> NestedTypes
        {
            get
            {
                if (_nestedTypes == null)
                {
                    _nestedTypes = _assemblyHydrator.ReadTypeNestedTypes(_typeDefinition);
                }
                return _nestedTypes;
            }
        }

        public IAssemblyMetadata Assembly { get; }
    }
}