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

        internal string ReadAssemblyCulture(AssemblyDefinition assemblyDefinition)
        {
            return _mdReader.GetString(assemblyDefinition.Culture);
        }

        internal string ReadAssemblyCulture(AssemblyReference assemblyReference)
        {
            return _mdReader.GetString(assemblyReference.Culture);
        }

        internal string ReadAssemblyName(AssemblyDefinition assemblyDefinition)
        {
            return _mdReader.GetString(assemblyDefinition.Name);
        }

        internal IReadOnlyCollection<byte> ReadAssemblyPublicKey(AssemblyDefinition assemblyDefinition)
        {
            return _mdReader.GetBlobContent(assemblyDefinition.PublicKey);
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

        internal IReadOnlyDictionary<string, IAssemblyMetadata> ReadRequiredAssemblies()
        {
            var requiredAssemblies = new Dictionary<string, IAssemblyMetadata>();
            foreach (AssemblyReferenceHandle asmRefHandle in _mdReader.AssemblyReferences)
            {
                AssemblyReference assemblyReference = _mdReader.GetAssemblyReference(asmRefHandle);
                string assemblyName = _mdReader.GetString(assemblyReference.Name);

                // TODO: Add the assembly resolver in here
                var lazyAssemblyReference = new LazyAssemblyReferenceMetadata(assemblyName, assemblyReference, this);
                requiredAssemblies.Add(assemblyName, lazyAssemblyReference);
            }

            return requiredAssemblies.ToImmutableDictionary();
        }

        internal IReadOnlyDictionary<string, ITypeMetadata> ReadDefinedTypes()
        {
            return ReadTypeDefinitions(_mdReader.TypeDefinitions, skipNested: true);
        }

        internal IReadOnlyDictionary<string, ITypeMetadata> ReadTypeDefinitions(
            IEnumerable<TypeDefinitionHandle> tdHandles,
            ITypeMetadata enclosingType = null,
            bool skipNested = false)
        {
            var definedTypes = new Dictionary<string, ITypeMetadata>();
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
            ITypeMetadata enclosingType = null)
        {
            string name = _mdReader.GetString(typeDefinition.Name);

            string @namespace = typeDefinition.Namespace.IsNil
                ? null
                : _mdReader.GetString(typeDefinition.Namespace);

            string fullName = enclosingType == null
                ? GetTypeFullName(name, @namespace)
                : GetTypeFullName(name, enclosingType);

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


        internal ITypeMetadata ReadPossiblyNestedTypeDefinition(TypeDefinition typeDefintion)
        {
            if (!typeDefintion.IsNested)
            {
                return ReadTypeDefinition(typeDefintion);
            }

            throw new NotImplementedException("Cannot read nested type without context yet");
        }

        internal ITypeMetadata ReadBaseType(TypeDefinition typeDefinition)
        {
            if (typeDefinition.BaseType.IsNil)
            {
                return null;
            }

            switch (typeDefinition.BaseType.Kind)
            {
            case HandleKind.TypeDefinition:
                TypeDefinition baseTypeDefinition = _mdReader.GetTypeDefinition((TypeDefinitionHandle)typeDefinition.BaseType);
                return ReadPossiblyNestedTypeDefinition(baseTypeDefinition);

            case HandleKind.TypeReference:
                TypeReference baseTypeReference = _mdReader.GetTypeReference((TypeReferenceHandle)typeDefinition.BaseType);
                throw new NotImplementedException($"Type reference resolution not yet implemented");

            default:
                throw new Exception($"Unable to process base type handle: {typeDefinition.BaseType}");
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

        internal IReadOnlyDictionary<string, IFieldMetadata> ReadTypeFields(TypeDefinition typeDefinition)
        {
            return ReadFields(typeDefinition.GetFields());
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

        }

        internal IReadOnlyDictionary<string, IReadOnlyCollection<IPropertyMetadata>> ReadTypeProperties(TypeDefinition typeDefinition)
        {

        }

        internal IReadOnlyDictionary<string, IReadOnlyCollection<IMethodMetadata>> ReadTypeMethods(TypeDefinition typeDefintion)
        {

        }

        internal IReadOnlyDictionary<string, ITypeMetadata> ReadTypeNestedTypes(TypeDefinition typeDefinition)
        {

        }

        #endregion /* Assembly reading methods */

        #region Static methods

        private static string GetTypeFullName(string name, string @namespace)
        {
            if (@namespace == null)
            {
                return name;
            }

            return new StringBuilder()
                .Append(@namespace)
                .Append('.')
                .Append(name)
                .ToString();
        }

        private static string GetTypeFullName(string name, ITypeMetadata enclosingType)
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

            case TypeAttributes.NestedPrivate:
            case TypeAttributes.NotPublic:
                return Accessibility.Private;

            default:
                throw new Exception($"Unknown type visibility attributes: {typeAttributes}");
            }
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