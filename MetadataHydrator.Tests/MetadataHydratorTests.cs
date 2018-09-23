using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Xunit;

namespace MetadataHydrator.Tests
{
    public abstract class MetadataHydratorTests
    {
        protected Assembly _assembly;

        protected IAssemblyDefinitionMetadata _assemblyMetadata;

        protected MetadataHydratorTests(Assembly testAsm, IAssemblyDefinitionMetadata assemblyMetadata)
        {
            _assemblyMetadata = assemblyMetadata;
            _assembly = testAsm;
        }

        [Fact]
        public void HasCorrectName()
        {
            Assert.Equal(_assembly.GetName().Name, _assemblyMetadata.Name);
        }

        [Fact]
        public void HasCorrectVersion()
        {
            Assert.Equal(_assembly.GetName().Version, _assemblyMetadata.Version);
        }

        [Fact]
        public void HasCorrectCulture()
        {
            Assert.Equal(_assembly.GetName().CultureName, _assemblyMetadata.Culture);
        }

        [Fact]
        public void HasCorrectPublicKey()
        {
            Assert.Equal(_assembly.GetName().GetPublicKey(), _assemblyMetadata.PublicKey);
        }

        [Fact]
        public void HasCorrectHashAlgorithm()
        {
            // The types are different, but should have the same names and values
            Assert.Equal((AssemblyHashAlgorithm)(int)_assembly.GetName().HashAlgorithm, _assemblyMetadata.HashAlgorithm);
            Assert.Equal(_assembly.GetName().HashAlgorithm.ToString(), _assemblyMetadata.HashAlgorithm.ToString(), ignoreCase: true);
        }
    }
}