using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator.Lazy.LoadedTypes
{
    public class LoadedTypeMetadata : ITypeDefinitionMetadata
    {
        private readonly Type _type;

        public LoadedTypeMetadata(Type type)
        {
            _type = type;
        }

        public Accessibility Accessibility
        {
            get
            {
                switch (_type.Attributes & TypeAttributes.VisibilityMask)
                {
                    case TypeAttributes.Public:
                    case TypeAttributes.NestedPublic:
                        return Accessibility.Public;

                    case TypeAttributes.NestedFamANDAssem:
                        return Accessibility.ProtectedAndInternal;

                    case TypeAttributes.NestedFamORAssem:
                        return Accessibility.ProtectedOrInternal;

                    case TypeAttributes.NestedAssembly:
                    case TypeAttributes.NotPublic:
                        return Accessibility.Internal;

                    case TypeAttributes.NestedFamily:
                        return Accessibility.Protected;

                    default:
                        throw new Exception($"Unknown visibility attribute on loaded type: {_type.Attributes}");
                }
            }
        }

        public ITypeReferenceMetadata BaseType => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, IFieldMetadata> Fields => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, IReadOnlyCollection<IPropertyMetadata>> Properties => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, IReadOnlyCollection<IMethodMetadata>> Methods => throw new System.NotImplementedException();

        public IReadOnlyDictionary<string, ITypeDefinitionMetadata> NestedTypes => throw new System.NotImplementedException();

        public IReadOnlyCollection<ICustomAttributeMetadata> CustomAttributes => throw new System.NotImplementedException();

        public IReadOnlyCollection<IGenericParameterMetadata> GenericParameters => throw new System.NotImplementedException();

        public IAssemblyDefinitionMetadata Assembly => throw new System.NotImplementedException();

        public string Name => throw new System.NotImplementedException();

        public string Namespace => throw new System.NotImplementedException();

        public string FullName => throw new System.NotImplementedException();
    }
}