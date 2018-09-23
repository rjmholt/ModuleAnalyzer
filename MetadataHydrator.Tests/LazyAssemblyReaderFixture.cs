using System;
using System.IO;
using System.Reflection;
using MetadataHydrator.Lazy;
using Xunit;

namespace MetadataHydrator.Tests
{
    public class LazyAssemblyReaderFixture
    {
        #region Static path variables

        private static readonly string s_currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static readonly string s_projRootPath = Path.Combine(s_currentPath, "..", "..", "..");

        private static readonly string s_assetsDirPath = Path.GetFullPath(Path.Combine(s_projRootPath, "assets"));

        private static readonly string s_asmPath = Path.Combine(s_assetsDirPath, "test.dll");

        #endregion /* Static path variables */

        private readonly LazyMetadataHydrator _metadataHydrator;

        private readonly Lazy<IAssemblyDefinitionMetadata> _assemblyHydrator;

        private readonly Lazy<Assembly> _assembly;

        public LazyAssemblyReaderFixture()
        {
            _metadataHydrator = new LazyMetadataHydrator();
            _assemblyHydrator = new Lazy<IAssemblyDefinitionMetadata>(() => _metadataHydrator.ReadAssembly(s_asmPath));
            _assembly = new Lazy<Assembly>(() => Assembly.LoadFile(s_asmPath));
        }

        public IAssemblyDefinitionMetadata TestLazyAssembly => _assemblyHydrator.Value;

        public Assembly TestAssembly => _assembly.Value;
    }

    [CollectionDefinition("LazyAsmReaderCollection")]
    public class LazyAssemblyReaderCollection : ICollectionFixture<LazyAssemblyReaderFixture>
    {
    }
}