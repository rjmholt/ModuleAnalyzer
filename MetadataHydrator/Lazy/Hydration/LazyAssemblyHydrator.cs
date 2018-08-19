using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using MetadataHydrator.Lazy.Metadata;

namespace MetadataHydrator.Lazy
{
    internal class LazyAssemblyHydrator : IDisposable
    {
        public static LazyAssemblyHydrator Create(
            string assemblyPath,
            TypeResolver typeResolver,
            AssemblyCache assemblyCache)
        {
            var memoryStream = new MemoryStream();
            using (Stream assemblyStream = File.OpenRead(assemblyPath))
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
                typeResolver,
                assemblyCache,
                assemblyPath);
        }

        private readonly TypeResolver _typeResolver;

        private readonly MetadataReader _mdReader;

        private readonly AssemblyCache _assemblyCache;

        private readonly PEReader _peReader;

        private MemoryStream _assemblyStream;

        internal LazyAssemblyHydrator(
            MemoryStream assemblyStream,
            PEReader peReader,
            MetadataReader mdReader,
            TypeResolver typeResolver,
            AssemblyCache assemblyCache,
            string assemblyPath)
        {
            _assemblyStream = assemblyStream;
            _peReader = peReader;
            _typeResolver = typeResolver;
            AssemblyPath = assemblyPath;
            _assemblyCache = assemblyCache;
            _mdReader = mdReader;
        }
        
        public LazyAssemblyDefinitionMetadata ReadAssembly()
        {
            var assemblyMetadata = new LazyAssemblyDefinitionMetadata(
                _mdReader.GetAssemblyDefinition(),
                this);

            _assemblyCache.AddAssembly(assemblyMetadata);

            return assemblyMetadata;
        }

        internal string AssemblyPath { get; }

        internal string GetAssemblyCulture(AssemblyDefinition assemblyDefinition)
        {
            return _mdReader.GetString(assemblyDefinition.Culture);
        }

        internal string GetAssemblyName(AssemblyDefinition assemblyDefinition)
        {
            return _mdReader.GetString(assemblyDefinition.Name);
        }

        internal IReadOnlyCollection<byte> GetAssemblyPublicKey(AssemblyDefinition assemblyDefinition)
        {
            return _mdReader.GetBlobContent(assemblyDefinition.PublicKey);
        }

        internal IReadOnlyCollection<LazyCustomAttributeMetadata> GetCustomAttributes(IEnumerable<CustomAttributeHandle> caHandles)
        {
            var customAttributes = new List<LazyCustomAttributeMetadata>();
            foreach (CustomAttributeHandle caHandle in caHandles)
            {
                CustomAttribute customAttribute = _mdReader.GetCustomAttribute(caHandle);
                customAttributes.Add(new LazyCustomAttributeMetadata(customAttribute, this));
            }

            return customAttributes.ToImmutableArray();
        }

        internal IReadOnlyDictionary<string, IAssemblyMetadata> GetRequiredAssemblies()
        {
            var requiredAssemblies = new Dictionary<string, IAssemblyMetadata>();
            foreach (AssemblyReferenceHandle asmRefHandle in _mdReader.AssemblyReferences)
            {
                AssemblyReference assemblyReference = _mdReader.GetAssemblyReference(asmRefHandle);
                string assemblyName = _mdReader.GetString(assemblyReference.Name);

                if (_assemblyCache.TryGetAssemblyByName(assemblyName, out IAssemblyMetadata assembly))
                {
                    requiredAssemblies.Add(assemblyName, assembly);
                    continue;
                }

                var lazyAssemblyReference = new LazyAssemblyReferenceMetadata(assemblyName, assemblyReference, this);
                _assemblyCache.AddAssembly(lazyAssemblyReference);
                requiredAssemblies.Add(assemblyName, lazyAssemblyReference);
            }

            return requiredAssemblies.ToImmutableDictionary();
        }

        internal IReadOnlyDictionary<string, ITypeMetadata> GetDefinedTypes()
        {
            var definedTypes = new Dictionary<string, ITypeMetadata>();
            foreach (TypeDefinitionHandle tdHandle in _mdReader.TypeDefinitions)
            {
                TypeDefinition typeDefinition = _mdReader.GetTypeDefinition(tdHandle);
                var typeMetadata = new LazyTypeMetadata(typeDefinition, this);
                _typeResolver.AddType(typeMetadata);
                definedTypes.Add(typeMetadata.FullName, typeMetadata);
            }

            return definedTypes.ToImmutableDictionary();
        }

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