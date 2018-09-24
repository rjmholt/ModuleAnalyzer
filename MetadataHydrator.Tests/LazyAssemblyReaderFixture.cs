using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MetadataHydrator.Lazy;
using Xunit;

namespace MetadataHydrator.Tests
{
    public class LazyAssemblyReaderFixture
    {
        private readonly LazyMetadataHydrator _metadataHydrator;

        private readonly Lazy<IAssemblyDefinitionMetadata> _assemblyHydrator;

        public LazyAssemblyReaderFixture()
        {
            _metadataHydrator = new LazyMetadataHydrator();
            _assemblyHydrator = new Lazy<IAssemblyDefinitionMetadata>(() => _metadataHydrator.ReadAssembly(TestDll.TestAssembly.Location));
        }

        public IAssemblyDefinitionMetadata TestLazyAssembly => _assemblyHydrator.Value;
    }

    [CollectionDefinition("LazyAsmReaderCollection")]
    public class LazyAssemblyReaderCollection : ICollectionFixture<LazyAssemblyReaderFixture>
    {
    }
}