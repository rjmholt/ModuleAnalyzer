using System.Reflection;
using Xunit;

namespace MetadataHydrator.Tests
{
    public abstract class MetadataHydratorTests
    {
        protected Assembly _assembly;

        protected IMetadataHydrator _metadataHydrator;

        protected MetadataHydratorTests(IMetadataHydrator metadataHydrator)
        {
            _metadataHydrator = metadataHydrator;
            _assembly = DllUtility.CompileSource();
        }

        [Fact]
        public void CanLoadAssembly()
        {
            Assert.True(true);
        }
    }
}