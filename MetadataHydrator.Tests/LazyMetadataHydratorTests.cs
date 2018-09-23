using MetadataHydrator.Lazy;
using Xunit;

namespace MetadataHydrator.Tests
{
    [Collection("LazyAsmReaderCollection")]
    public class LazyMetadataHydratorTests : MetadataHydratorTests
    {
        public LazyMetadataHydratorTests(LazyAssemblyReaderFixture lazyAsmReaderFixture)
            : base(lazyAsmReaderFixture.TestAssembly, lazyAsmReaderFixture.TestLazyAssembly)
        {
        }
    }
}