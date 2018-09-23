using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using MetadataHydrator.Lazy.Metadata;
using Microsoft.CodeAnalysis;

namespace MetadataHydrator.Lazy
{
    internal class LazyAssemblyHydrator : IDisposable
    {
        public static LazyAssemblyHydrator Create(
            FileInfo assemblyFile,
            AssemblyDirectoryResolver assemblyDirectoryResolver)
        {
            var memoryStream = new MemoryStream();
            using (Stream assemblyStream = assemblyFile.OpenRead())
            {
                assemblyStream.CopyTo(memoryStream);
            }
            memoryStream.Position = 0;

            var peReader = new PEReader(memoryStream);
            MetadataReader mdReader = peReader.GetMetadataReader();

            return new LazyAssemblyHydrator(
                memoryStream,
                peReader,
                mdReader,
                assemblyDirectoryResolver,
                assemblyFile);
        }

        private readonly MetadataReader _mdReader;

        private readonly AssemblyDirectoryResolver _asmDirResolver;

        private readonly PEReader _peReader;

        private MemoryStream _assemblyStream;

        private LazyAssemblyDefinitionMetadata _assemblyMetadata;

        internal LazyAssemblyHydrator(
            MemoryStream assemblyStream,
            PEReader peReader,
            MetadataReader mdReader,
            AssemblyDirectoryResolver asmDirResolver,
            FileInfo assemblyFile)
        {
            _assemblyStream = assemblyStream;
            _peReader = peReader;
            _mdReader = mdReader;
            _asmDirResolver = asmDirResolver;
            AssemblyFile = assemblyFile;
        }

        internal LazyAssemblyDefinitionMetadata Assembly
        {
            get
            {
                if (_assemblyMetadata == null)
                {
                    var assemblyMetadata = new LazyAssemblyDefinitionMetadata(
                        _mdReader.GetAssemblyDefinition(),
                        this);

                    _assemblyMetadata = assemblyMetadata;
                }
                return _assemblyMetadata;
            }
        }
        
        public LazyAssemblyDefinitionMetadata ReadAssembly()
        {
            return Assembly;
        }

        #region Properties

        internal FileInfo AssemblyFile { get; }

        #endregion /* Properties */

        #region Assembly reading methods

        internal string ReadString(StringHandle strHandle)
        {
            return _mdReader.GetString(strHandle);
        }

        internal IReadOnlyCollection<byte> ReadBlob(BlobHandle blobHandle)
        {
            return _mdReader.GetBlobContent(blobHandle);
        }

        internal IReadOnlyCollection<LazyCustomAttributeMetadata> ReadCustomAttributes(IEnumerable<CustomAttributeHandle> caHandles)
        {
            var customAttributes = new List<LazyCustomAttributeMetadata>();
            foreach (CustomAttributeHandle caHandle in caHandles)
            {
                CustomAttribute customAttribute = _mdReader.GetCustomAttribute(caHandle);
                customAttributes.Add(new LazyCustomAttributeMetadata(customAttribute, this));
            }

            return customAttributes.ToImmutableArray();
        }

        internal IReadOnlyCollection<LazyGenericParameterMetadata> ReadGenericParameters(IEnumerable<GenericParameterHandle> gpHandles)
        {
            var genericParameters = new List<LazyGenericParameterMetadata>();
            foreach (GenericParameterHandle gpHandle in gpHandles)
            {
                GenericParameter genericParameter = _mdReader.GetGenericParameter(gpHandle);
                genericParameters.Add(new LazyGenericParameterMetadata(genericParameter, this));
            }

            return genericParameters;
        }

        internal IReadOnlyDictionary<string, IAssemblyDefinitionMetadata> ReadReferencedAssemblies()
        {
            return ReadAssemblyReferences(_mdReader.AssemblyReferences);
        }

        internal IReadOnlyDictionary<string, IAssemblyDefinitionMetadata> ReadAssemblyReferences(IEnumerable<AssemblyReferenceHandle> arHandles)
        {
            var requiredAssemblies = new Dictionary<string, IAssemblyDefinitionMetadata>();
            foreach (AssemblyReferenceHandle asmRefHandle in arHandles)
            {
                AssemblyReference assemblyReference = _mdReader.GetAssemblyReference(asmRefHandle);
                string assemblyName = _mdReader.GetString(assemblyReference.Name);

                // TODO: Add the assembly resolver in here
                var lazyAssemblyReference = new LazyAssemblyReferenceMetadata(assemblyName, assemblyReference, this);
                requiredAssemblies.Add(assemblyName, lazyAssemblyReference);
            }

            return requiredAssemblies.ToImmutableDictionary();
        }

        internal IReadOnlyDictionary<string, ITypeDefinitionMetadata> ReadDefinedTypes()
        {
            return ReadTypeDefinitions(_mdReader.TypeDefinitions, skipNested: true);
        }

        internal IReadOnlyDictionary<string, ITypeDefinitionMetadata> ReadTypeDefinitions(
            IEnumerable<TypeDefinitionHandle> tdHandles,
            ITypeDefinitionMetadata enclosingType = null,
            bool skipNested = false)
        {
            var definedTypes = new Dictionary<string, ITypeDefinitionMetadata>();
            foreach (TypeDefinitionHandle tdHandle in tdHandles)
            {
                if (tdHandle.IsNil)
                {
                    continue;
                }

                TypeDefinition typeDefinition = _mdReader.GetTypeDefinition(tdHandle);
                if (skipNested && typeDefinition.IsNested)
                {
                    continue;
                }

                LazyTypeDefinitionMetadata typeMetadata = ReadTypeDefinition(typeDefinition, enclosingType);
                definedTypes.Add(typeMetadata.FullName, typeMetadata);
            }

            return definedTypes.ToImmutableDictionary();
        }

        internal LazyTypeDefinitionMetadata ReadTypeDefinition(
            TypeDefinition typeDefinition,
            ITypeDefinitionMetadata enclosingType = null)
        {
            string fullName = ReadTypeFullName(typeDefinition, out string name, out string @namespace, enclosingType);

            Accessibility accessibility = GetAccessibilityFromAttributes(typeDefinition.Attributes);

            var typeMetadata = new LazyTypeDefinitionMetadata(
                typeDefinition,
                Assembly,
                name,
                @namespace,
                fullName,
                accessibility,
                this);
            
            return typeMetadata;
        }


        internal ITypeDefinitionMetadata ReadPossiblyNestedTypeDefinition(TypeDefinition typeDefintion)
        {
            if (!typeDefintion.IsNested)
            {
                return ReadTypeDefinition(typeDefintion);
            }

            throw new NotImplementedException("Cannot read nested type without context yet");
        }

        internal ITypeDefinitionMetadata ReadTypeFromHandle(EntityHandle handle)
        {
            if (handle.IsNil)
            {
                return null;
            }

            switch (handle.Kind)
            {
            case HandleKind.TypeDefinition:
                TypeDefinition baseTypeDefinition = _mdReader.GetTypeDefinition((TypeDefinitionHandle)handle);
                return ReadPossiblyNestedTypeDefinition(baseTypeDefinition);

            case HandleKind.TypeReference:
                return ResolveType((TypeReferenceHandle)handle);

            default:
                throw new Exception($"Unable to process type handle: {handle}");
            }
        }

        internal string ReadTypeName(TypeDefinition typeDefinition)
        {
            return _mdReader.GetString(typeDefinition.Name);
        }

        internal string ReadTypeNamespace(TypeDefinition typeDefinition)
        {
            return _mdReader.GetString(typeDefinition.Namespace);
        }

        internal IReadOnlyDictionary<string, IFieldMetadata> ReadFields(IEnumerable<FieldDefinitionHandle> fdHandles)
        {
            var fields = new Dictionary<string, IFieldMetadata>();
            foreach (FieldDefinitionHandle fdHandle in fdHandles)
            {
                LazyFieldMetadata field = ReadField(fdHandle);
                fields.Add(field.Name, field);
            }
            return fields.ToImmutableDictionary();
        }

        internal LazyFieldMetadata ReadField(FieldDefinitionHandle fdHandle)
        {
            FieldDefinition fieldDefinition = _mdReader.GetFieldDefinition(fdHandle);
            string name = _mdReader.GetString(fieldDefinition.Name);
            Accessibility accessibility = GetAccessibilityFromAttributes(fieldDefinition.Attributes);
            return new LazyFieldMetadata(name, accessibility, fieldDefinition, this);
        }

        internal IReadOnlyDictionary<string, IReadOnlyCollection<IPropertyMetadata>> ReadProperties(IEnumerable<PropertyDefinitionHandle> propertyHandles)
        {
            var properties = new Dictionary<string, List<IPropertyMetadata>>();
            foreach (PropertyDefinitionHandle propertyHandle in propertyHandles)
            {
                IPropertyMetadata property = ReadProperty(propertyHandle);
                if (!properties.ContainsKey(property.Name))
                {
                    properties.Add(property.Name, new List<IPropertyMetadata>());
                }
                properties[property.Name].Add(property);
            }
            return MakeListTableImmutable(properties);
        }

        internal IPropertyMetadata ReadProperty(PropertyDefinitionHandle propertyHandle)
        {
            PropertyDefinition propertyDefinition = _mdReader.GetPropertyDefinition(propertyHandle);
            string name = _mdReader.GetString(propertyDefinition.Name);
            return new LazyPropertyMetadata(name, propertyDefinition, this);
        }

        internal IReadOnlyDictionary<string, IReadOnlyCollection<IMethodMetadata>> ReadMethods(IEnumerable<MethodDefinitionHandle> methodHandles)
        {
            var methods = new Dictionary<string, List<IMethodMetadata>>();
            foreach (MethodDefinitionHandle methodHandle in methodHandles)
            {
                IMethodMetadata method = ReadMethod(methodHandle);
                if (!methods.ContainsKey(method.Name))
                {
                    methods.Add(method.Name, new List<IMethodMetadata>());
                }
                methods[method.Name].Add(method);
            }
            return MakeListTableImmutable(methods);
        }

        internal IMethodMetadata ReadMethod(MethodDefinitionHandle methodHandle)
        {
            MethodDefinition methodDefinition = _mdReader.GetMethodDefinition(methodHandle);
            string name = _mdReader.GetString(methodDefinition.Name);
            Accessibility accessibility = GetAccessibilityFromAttributes(methodDefinition.Attributes);
            return new LazyMethodMetadata(name, accessibility, methodDefinition, this);
        }

        private string ReadTypeFullName(TypeDefinition typeDefinition, ITypeDefinitionMetadata enclosingType = null)
        {
            return ReadTypeFullName(typeDefinition, out string name, out string @namespace, enclosingType);
        }

        private string ReadTypeFullName(TypeDefinition typeDefinition, out string name, out string @namespace, ITypeDefinitionMetadata enclosingType = null)
        {
            name = _mdReader.GetString(typeDefinition.Name);
            @namespace = typeDefinition.Namespace.IsNil
                ? null
                : _mdReader.GetString(typeDefinition.Namespace);

            if (enclosingType == null)
            {
                return GetTypeFullName(name, @namespace);
            }

            return GetTypeFullName(name, enclosingType);
        }

        #endregion /* Assembly reading methods */

        #region Type resolution hooks

        internal IReadOnlyDictionary<string, ITypeDefinitionMetadata> ReadTypeReferences()
        {
            return ResolveTypeReferences(_mdReader.TypeReferences);
        }

        internal IReadOnlyDictionary<string, ITypeDefinitionMetadata> ResolveTypeReferences(IEnumerable<TypeReferenceHandle> trHandles)
        {
            var typeReferences = new Dictionary<string, ITypeDefinitionMetadata>();
            foreach (TypeReferenceHandle trHandle in trHandles)
            {
                ITypeDefinitionMetadata typeReference = ResolveType(trHandle);
                typeReferences.Add(typeReference.FullName, typeReference);
            }
            return typeReferences.ToImmutableDictionary();
        }

        internal ITypeDefinitionMetadata ResolveType(TypeReferenceHandle trHandle)
        {
            // TODO: Search assembly, then search the dir resolver, then search the loaded type resolver
            throw new NotImplementedException();
        }

        internal ITypeDefinitionMetadata ResolveFieldType(FieldDefinition fieldDefinition)
        {
            throw new NotImplementedException();
            //return fieldDefinition.DecodeSignature();
        }

        #endregion /* Type resolution hooks */

        #region Static methods

        private static string GetTypeFullName(string name, string @namespace)
        {
            if (String.IsNullOrEmpty(@namespace))
            {
                return name;
            }

            return new StringBuilder()
                .Append(@namespace)
                .Append('.')
                .Append(name)
                .ToString();
        }

        private static string GetTypeFullName(string name, ITypeDefinitionMetadata enclosingType)
        {
            return new StringBuilder()
                .Append(enclosingType.FullName)
                .Append('.')
                .Append(name)
                .ToString();
        }

        private static Accessibility GetAccessibilityFromAttributes(TypeAttributes typeAttributes)
        {
            switch (typeAttributes & TypeAttributes.VisibilityMask)
            {
            case TypeAttributes.Public:
            case TypeAttributes.NestedPublic:
                return Accessibility.Public;

            case TypeAttributes.NestedFamANDAssem:
                return Accessibility.ProtectedAndInternal;

            case TypeAttributes.NestedFamORAssem:
                return Accessibility.ProtectedOrInternal;

            case TypeAttributes.NestedAssembly:
                return Accessibility.Internal;

            case TypeAttributes.NestedFamily:
                return Accessibility.Protected;

            case TypeAttributes.NestedPrivate:
            case TypeAttributes.NotPublic:
                return Accessibility.Private;

            default:
                throw new Exception($"Unknown type visibility attributes: {typeAttributes}");
            }
        }

        private static Accessibility GetAccessibilityFromAttributes(FieldAttributes fieldAttributes)
        {
            switch (fieldAttributes & FieldAttributes.FieldAccessMask)
            {
            case FieldAttributes.Public:
                return Accessibility.Public;

            case FieldAttributes.FamANDAssem:
                return Accessibility.ProtectedAndInternal;

            case FieldAttributes.FamORAssem:
                return Accessibility.ProtectedOrInternal;

            case FieldAttributes.Assembly:
                return Accessibility.Internal;

            case FieldAttributes.Family:
                return Accessibility.Protected;

            case FieldAttributes.Private:
                return Accessibility.Private;

            default:
                throw new Exception($"Unknown field visibility attributes: {fieldAttributes}");
            }
        }

        private static Accessibility GetAccessibilityFromAttributes(MethodAttributes methodAttributes)
        {
            switch (methodAttributes & MethodAttributes.MemberAccessMask)
            {
            case MethodAttributes.Public:
                return Accessibility.Public;

            case MethodAttributes.FamANDAssem:
                return Accessibility.ProtectedAndInternal;

            case MethodAttributes.FamORAssem:
                return Accessibility.ProtectedOrInternal;

            case MethodAttributes.Assembly:
                return Accessibility.Internal;

            case MethodAttributes.Family:
                return Accessibility.Protected;

            case MethodAttributes.Private:
                return Accessibility.Private;

            default:
                throw new Exception($"Unknown method visibility attributes: {methodAttributes}");
            }
        }

        private static IReadOnlyDictionary<K, IReadOnlyCollection<V>> MakeListTableImmutable<K, V>(Dictionary<K, List<V>> table)
        {
            var dictionary = new Dictionary<K, IReadOnlyCollection<V>>();
            foreach (KeyValuePair<K, List<V>> entry in table)
            {
                dictionary.Add(entry.Key, entry.Value.ToImmutableArray());
            }
            return dictionary.ToImmutableDictionary();
        }

        #endregion /* Static methods */

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _peReader.Dispose();
                    _assemblyStream.Dispose();
                }
                _assemblyStream = null;
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}